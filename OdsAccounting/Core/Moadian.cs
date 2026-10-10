using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    /// <summary>
    /// ارسال فاکتور به سامانه مودیان (مؤدیان مالیاتی).
    /// نکته: ارسال واقعی نیازمند کلید/گواهی امضای دیجیتال و شناسه حافظه مالیاتی شرکت است؛
    /// این ماژول ساختار داده، ثبت لاگ و وضعیت فاکتور را مدیریت می‌کند.
    /// </summary>
    public static class MoadianService
    {
        public sealed class SendResult
        {
            public bool Success { get; set; }
            public int HttpStatus { get; set; }
            public string Reference { get; set; }
            public string Message { get; set; }
        }

        /// <summary>ساخت بسته JSON فاکتور مطابق ساختار header/body مودیان.</summary>
        public static string BuildPayload(int invoiceId, out string sellerTaxId)
        {
            var header = AppDb.Query(@"
                SELECT i.InvoiceNo, i.InvoiceDate, i.InvoiceType, i.SubTotal, i.Discount, i.VatAmount, i.TotalAmount,
                       c.NationalID AS SellerId, e.NationalID AS BuyerId
                FROM ods.SC_Invoices i
                JOIN ods.SC_Companies c ON c.CompanyID = i.CompanyID
                LEFT JOIN ods.SC_FloatingEntities e ON e.EntityId = i.EntityId
                WHERE i.InvoiceId = @id",
                new SqlParameter("@id", invoiceId));
            if (header.Rows.Count == 0) throw new InvalidOperationException("فاکتور یافت نشد.");
            DataRow h = header.Rows[0];
            sellerTaxId = Convert.ToString(h["SellerId"]);

            var lines = AppDb.Query(@"SELECT ItemCode, ItemName, Unit, Quantity, UnitPrice, Discount, VatRate, VatAmount, LineTotal
                                      FROM ods.SC_InvoiceLines WHERE InvoiceId = @id ORDER BY [LineNo]",
                new SqlParameter("@id", invoiceId));

            var body = new List<Dictionary<string, object>>();
            foreach (DataRow l in lines.Rows)
            {
                body.Add(new Dictionary<string, object>
                {
                    ["sstid"] = Convert.ToString(l["ItemCode"]),
                    ["sstt"] = Convert.ToString(l["ItemName"]),
                    ["mu"] = Convert.ToString(l["Unit"]),
                    ["am"] = Convert.ToDecimal(l["Quantity"]),
                    ["fee"] = Convert.ToDecimal(l["UnitPrice"]),
                    ["dis"] = Convert.ToDecimal(l["Discount"]),
                    ["vra"] = Convert.ToDecimal(l["VatRate"]),
                    ["vam"] = Convert.ToDecimal(l["VatAmount"]),
                    ["tsstam"] = Convert.ToDecimal(l["LineTotal"])
                });
            }

            var payload = new Dictionary<string, object>
            {
                ["header"] = new Dictionary<string, object>
                {
                    ["taxid"] = Guid.NewGuid().ToString("N").ToUpperInvariant(),
                    ["indatim"] = new DateTimeOffset(Convert.ToDateTime(h["InvoiceDate"])).ToUnixTimeMilliseconds(),
                    ["inty"] = Convert.ToInt32(h["InvoiceType"]),
                    ["inno"] = Convert.ToString(h["InvoiceNo"]),
                    ["tins"] = sellerTaxId,
                    ["bid"] = Convert.ToString(h["BuyerId"]),
                    ["tprdis"] = Convert.ToDecimal(h["SubTotal"]),
                    ["tdis"] = Convert.ToDecimal(h["Discount"]),
                    ["tvam"] = Convert.ToDecimal(h["VatAmount"]),
                    ["tbill"] = Convert.ToDecimal(h["TotalAmount"])
                },
                ["body"] = body
            };
            return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = false });
        }

        /// <summary>ارسال فاکتور؛ نتیجه در جدول لاگ و وضعیت فاکتور ثبت می‌شود.</summary>
        public static SendResult Send(int invoiceId)
        {
            string tin;
            string json = BuildPayload(invoiceId, out tin);
            string endpoint = Properties.Settings.Default.MoadianEndpoint;

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.MoadianTaxId) ||
                string.IsNullOrWhiteSpace(Properties.Settings.Default.MoadianCertThumbprint))
            {
                return Record(invoiceId, json, false, 0, null, "شناسه مؤدی یا اثرانگشت گواهی امضا در تنظیمات تکمیل نشده است.");
            }

            var result = SendAsync(endpoint, json).GetAwaiter().GetResult();
            return Record(invoiceId, json, result.Success, result.HttpStatus, result.Reference, result.Message);
        }

        private static async Task<SendResult> SendAsync(string endpoint, string json)
        {
            using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) })
            using (var content = new StringContent(json, Encoding.UTF8))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.Add("X-Cert-Thumbprint", Properties.Settings.Default.MoadianCertThumbprint);
                try
                {
                    var response = await http.PostAsync(endpoint, content);
                    string body = await response.Content.ReadAsStringAsync();
                    string reference = null;
                    try
                    {
                        using (var doc = JsonDocument.Parse(body))
                            if (doc.RootElement.TryGetProperty("referenceNumber", out var r)) reference = r.ToString();
                    }
                    catch (JsonException) { }
                    return new SendResult
                    {
                        Success = response.IsSuccessStatusCode,
                        HttpStatus = (int)response.StatusCode,
                        Reference = reference,
                        Message = body
                    };
                }
                catch (Exception ex)
                {
                    return new SendResult { Success = false, HttpStatus = 0, Message = ex.Message };
                }
            }
        }

        private static SendResult Record(int invoiceId, string json, bool success, int status, string reference, string message)
        {
            AppDb.InTransaction((conn, tran) =>
            {
                using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_MoadianLogs (InvoiceId, RequestJson, HttpStatus, Success, ResponseText, Message)
                                                  VALUES (@i, @j, @s, @ok, @r, @m)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@i", invoiceId);
                    cmd.Parameters.AddWithValue("@j", json);
                    cmd.Parameters.AddWithValue("@s", status);
                    cmd.Parameters.AddWithValue("@ok", success);
                    cmd.Parameters.AddWithValue("@r", (object)message ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@m", success ? "ارسال موفق" : (object)(message ?? "خطا") ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand(@"UPDATE ods.SC_Invoices SET Status = @st, MoadianReferenceNo = COALESCE(@ref, MoadianReferenceNo),
                                                  MoadianStatus = @ms WHERE InvoiceId = @i", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@st", success ? 1 : 4);
                    cmd.Parameters.AddWithValue("@ref", (object)reference ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ms", success ? "SENT" : "ERROR");
                    cmd.Parameters.AddWithValue("@i", invoiceId);
                    cmd.ExecuteNonQuery();
                }
            });
            return new SendResult { Success = success, HttpStatus = status, Reference = reference, Message = message };
        }
    }
}
