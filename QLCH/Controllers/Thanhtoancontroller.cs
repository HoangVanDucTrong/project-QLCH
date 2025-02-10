using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLCH.Data;
using QLCH.Models;
using QLCH.Models.IRepository;
using QRCoder;

namespace QLCH.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class Thanhtoancontroller : Controller
    {
        private readonly IGetClaimsFromToken _getClaimsFromToken;
        private readonly QLCHDbConText _context;
        public Thanhtoancontroller(QLCHDbConText context, ILogger<Thongtintaikhoan> logger, IGetClaimsFromToken getClaimsFromToken)
        {
            _context = context;
            _getClaimsFromToken = getClaimsFromToken;
        }
        /*
        [HttpPost("stores/createqr")]
        public IActionResult CreateQrForStore([FromBody] CreateTransactionRequest request)
        {
            // Lấy token từ Header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            // Lấy StoreId từ claims
            var storeIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeIdClaim))
            {
                return Unauthorized(); // Không có StoreId trong claims
            }

            if (!int.TryParse(storeIdClaim, out int storeId))
            {
                return BadRequest("Invalid StoreId"); // StoreId không hợp lệ
            }

         

            try
            {
                // Truy vấn thông tin cửa hàng từ database
                var store = _context.Stores.FirstOrDefault(s => s.StoreId == storeId);
                if (store == null)
                {
                    return NotFound("Store not found.");
                }

                // Lấy thông tin cần thiết
                string bankCode = "VBAAVNVX"; // Mã ngân hàng (thay đổi nếu cần)
                string bankAccount = store.BankAccount; // Tài khoản ngân hàng lấy từ database
                decimal amount = request.Transaction.Amount; // Số tiền cần thanh toán
                string note = $"StoreID_{storeId}_OrderID_{Guid.NewGuid()}"; // Nội dung giao dịch

                // Tạo nội dung QR code
                var qrContent = $"000201010211" +
                                $"0208{bankCode}" +
                                $"0301{bankAccount}" +
                                $"0401{amount:0}" +
                                $"0501{note}" +
                                $"6304"; // Checksum (cần tính toán nếu cần)

                // Tạo QR code
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);

                using (var bitmap = qrCode.GetGraphic(20))
                {
                    using (var stream = new MemoryStream())
                    {
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        var qrBase64 = Convert.ToBase64String(stream.ToArray());
                        var qrUrl = $"data:image/png;base64,{qrBase64}";

                        // Lưu mã QR vào database
                        var newStoreQr = new Thongtintaikhoan
                        {
                            StoreId = storeId,
                            BankAccount = bankAccount,
                            QRCodeUrl = qrUrl
                        };

                        _context.thongtintaikhoans.Add(newStoreQr);
                        _context.SaveChanges();

                        return Ok(new { success = true, qrCodeUrl = qrUrl });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        */
        [HttpPost("CreateTransaction")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTransaction([FromBody] transaction newTransaction)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }
            if (newTransaction == null)
            {
                return BadRequest(new { success = false, message = "Transaction data is required." });
            }

            try
            {
                newTransaction.StoreId = int.Parse(storeId);
                if (newTransaction.Amount <= 0)
                {
                    return BadRequest(new { success = false, message = " Amount must be provided and valid." });
                }
                // Thiết lập giá trị mặc định nếu chưa có
                newTransaction.CreatedAt = DateTime.Now; // Gán thời gian hiện tại
                newTransaction.Status = string.IsNullOrEmpty(newTransaction.Status) ? "Pending" : newTransaction.Status;
          
                // Lưu vào database
                _context.transactions.Add(newTransaction);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Transaction created successfully.",
                    data = newTransaction
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi bất ngờ
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        /*
        [HttpPost("api/payment/notify")]
        public IActionResult NotifyPayment([FromBody] PaymentNotificationModel notification)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var storeIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            try
            {
                // Giả sử nội dung chuyển khoản: "StoreID_TransactionID"
                var parts = notification.Note.Split('_');
                if (parts.Length < 2) return BadRequest("Invalid transaction note");

                var storeId = storeIdClaim;
                var transactionId = int.Parse(parts[1]);

                // Tìm giao dịch trong database
                var transaction = _context.Transactions.FirstOrDefault(t => t.TransactionId == transactionId && t.StoreId == storeId);
                if (transaction == null) return NotFound("Transaction not found");

                // Cập nhật trạng thái giao dịch
                transaction.Status = "Paid";
                _context.SaveChanges();

                return Ok(new { success = true, message = "Transaction updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        */
    }
}
