using Microsoft.AspNetCore.Mvc;
using QLCH.Data;
using QLCH.Models.IRepository;
using QLCH.Models;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;

namespace QLCH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThanhtoanClient : Controller
    {
        private readonly IGetClaimsFromToken _getClaimsFromToken;
        private readonly QLCHDbConText _context;
        private readonly IHubContext<TableHub> _hubContext;
        public ThanhtoanClient(QLCHDbConText context, ILogger<Thongtintaikhoan> logger, IGetClaimsFromToken getClaimsFromToken, IHubContext<TableHub> hubContext)
        {
            _context = context;
            _getClaimsFromToken = getClaimsFromToken;
            _hubContext = hubContext;
        }
        [HttpPost("CreateQrForStore")]
        public async Task<IActionResult> CreateQrForStore([FromBody] CreateTransactionRequest request)
        {
            try
            {
               

                // Kiểm tra giao dịch tồn tại
                var transaction = _context.transactions.FirstOrDefault(t => t.TransactionId == request.transactionId && t.StoreId == request.storeid);
                if (transaction == null)
                {
                    return NotFound(new { success = false, message = "Transaction not found." });
                }

                // Kiểm tra thông tin ngân hàng
                var bankInfo = _context.Thongtintaikhoan.FirstOrDefault(b => b.StoreId == request.storeid);
                if (bankInfo == null || string.IsNullOrEmpty(bankInfo.BankAccount))
                {
                    return NotFound(new { message = "Cửa hàng của bạn chưa thêm tài khoản ngân hàng!" });
                }

                // ✅ Chuẩn bị dữ liệu gửi lên VietQR API
                var apiRequest = new APIRequest
                {
                    acqId = bankInfo.AcqId, // Ví dụ: Mã ngân hàng techcombak (thay đổi theo ngân hàng)
                    accountNo = long.Parse(bankInfo.BankAccount), // Số tài khoản ngân hàng
                    accountName = bankInfo.AccountName, // Tên tài khoản ngân hàng
                    amount = (int)transaction.Amount, // Số tiền cần thanh toán
                    addInfo = $"Thanh toán đơn hàng {transaction.TransactionId}", // Nội dung giao dịch
                    format = "text",
                    template = "compact2"
                };

                // Chuyển request thành JSON
                var jsonRequest = JsonConvert.SerializeObject(apiRequest);

                // Gửi request đến API VietQR
                var client = new RestClient("https://api.vietqr.io/v2/generate"); // Đổi thành URL chính xác của VietQR
                var apiRequestRest = new RestRequest();
                apiRequestRest.Method = Method.Post;
                apiRequestRest.AddHeader("Accept", "application/json");
                apiRequestRest.AddHeader("Content-Type", "application/json");
                apiRequestRest.AddParameter("application/json", jsonRequest, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(apiRequestRest);
                if (!response.IsSuccessful)
                {
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Không thể tạo mã QR từ VietQR." });
                }

                // Xử lý phản hồi từ API VietQR
                var dataResult = JsonConvert.DeserializeObject<APIResponse>(response.Content);
                if (dataResult == null || dataResult.data == null || string.IsNullOrEmpty(dataResult.data.qrDataURL))
                {
                    return BadRequest(new { success = false, message = "Dữ liệu phản hồi không hợp lệ từ VietQR." });
                }

                //  Lưu URL mã QR vào database
                transaction.QRCodeUrl = dataResult.data.qrDataURL;
                _context.transactions.Update(transaction);
                await _context.SaveChangesAsync();

                //  Trả về URL của mã QR để frontend hiển thị
                return Ok(new { success = true, qrCodeUrl = dataResult.data.qrDataURL });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost("CreateTransaction")]
        public async Task<IActionResult> CreateTransaction([FromBody] transaction newTransaction)

        {
            if (newTransaction == null)
            {
                return BadRequest(new { success = false, message = "Transaction data is required." });
            }

            try
            {
                if (newTransaction.Amount <= 0)
                {
                    return BadRequest(new { success = false, message = "Amount must be provided and valid." });
                }

                if (newTransaction.StoreId <= 0)
                {
                    return BadRequest(new { success = false, message = "StoreId is required." });
                }
                if (newTransaction.BanId <= 0)  // Kiểm tra bàn
                {
                    return BadRequest(new { success = false, message = "Bàn không hợp lệ." });
                }
                // Thiết lập giá trị mặc định
               // newTransaction.CreatedAt = DateTime.Now;
             //   newTransaction.Status = string.IsNullOrEmpty(newTransaction.Status) ? "Pending" : newTransaction.Status;

                // Lưu vào database
             
                // 🔴 Cập nhật trạng thái bàn
                var table = await _context.bans.FirstOrDefaultAsync(b => b.BanId == newTransaction.BanId && b.StoreId == newTransaction.StoreId);
                if (table != null)
                {
                    table.IsInUse = "true"; // Đánh dấu bàn đã có khách
                    await _hubContext.Clients.All.SendAsync("ReceiveTableUpdate", newTransaction.BanId, table.IsInUse);
                }
                var transactionToSave = new transaction
                {
                  StoreId = newTransaction.StoreId,
                  Amount = newTransaction.Amount,
                  QRCodeUrl = newTransaction.QRCodeUrl,
                  Status = newTransaction.Status ?? "Pending",
                  CreatedAt = DateTime.Now,
                  Note = newTransaction.Note
                };
                _context.transactions.Add(transactionToSave);
                await _context.SaveChangesAsync();
                Console.WriteLine("Transaction ID vừa tạo:", transactionToSave.TransactionId);
                return Ok(new
                {
                    success = true,
                    message = "Transaction created successfully.",
                    transactionId = transactionToSave.TransactionId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }
        [HttpPost("statusBan")]
        public async Task<IActionResult> statusBan([FromBody] TableStatusRequest request)
        {

            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Không nhận được dữ liệu từ frontend." });
                }

                if (request.BanId <= 0 || request.StoreId <= 0)
                {
                    return BadRequest(new { success = false, message = "Số bàn hoặc số cửa hàng không hợp lệ." });
                }

                // 🔴 Cập nhật trạng thái bàn
                var table = await _context.bans.FirstOrDefaultAsync(b => b.BanId == request.BanId && b.StoreId == request.StoreId);
                if (table != null)
                {
                    table.IsInUse = table.IsInUse == "true" ? "false" : "true";
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("ReceiveTableUpdate", request.BanId, table.IsInUse);
                }

                return Ok(new
                {
                    success = true,
                    message = "Bàn đã được cập nhật trạng thái."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            try
            {

                if (orderRequest == null || orderRequest.Products == null || orderRequest.Products.Count == 0)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu đơn hàng không hợp lệ." });
                }

                // Lưu đơn hàng mới
                var newOrder = new ChiTietDonHang
                {
                    StoreId = orderRequest.StoreId,
                    BanId = orderRequest.BanId,     
                    ImageCheckBank = orderRequest.ImageCheckBank, // Ảnh dạng Base64
                    NgayTao = DateTime.Now,
                   
                };

                _context.ChiTietDonHangs.Add(newOrder);
                await _context.SaveChangesAsync();

                // Lưu danh sách sản phẩm vào bảng `SanPhamDonHang`
                foreach (var product in orderRequest.Products)
                {
                    var orderProduct = new SanPhamDonHang
                    {
                        CTDHId = newOrder.CTDHId,
                        SanPhamId = product.SanPhamId,
                        SoLuong = product.SoLuong,
                        Gia = product.Gia,
                        TongTien = product.SoLuong * product.Gia,
             
                    };

                    _context.sanPhamDonHangs.Add(orderProduct);
                }

                await _context.SaveChangesAsync();
                // 🔴 Gửi thông báo đến tất cả client đã kết nối SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", new
                {
                    message = $"📢 Có đơn hàng mới từ bàn {orderRequest.BanId}!",
                    banId = orderRequest.BanId
                });

                return Ok(new { success = true, message = "Đơn hàng đã được tạo thành công!", orderId = newOrder.CTDHId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }

    }
}
