
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using QLCH.Data;
using QLCH.Models;
using QLCH.Models.IRepository;
using QRCoder;
using System;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using RestSharp;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost("stores/createqr")]
        public async Task<IActionResult> CreateQrForStore([FromBody] CreateTransactionRequest request)
        {
            try
            {
                // Lấy token từ Header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var claimsPrincipal = _getClaimsFromToken.laytoken(token);

                // Lấy StoreId từ claims
                var storeIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
                if (string.IsNullOrEmpty(storeIdClaim) || !int.TryParse(storeIdClaim, out int storeId))
                {
                    return Unauthorized(new { success = false, message = "Invalid StoreId" });
                }

                // Kiểm tra giao dịch tồn tại
                var transaction = _context.transactions.FirstOrDefault(t => t.TransactionId == request.transactionId && t.StoreId == storeId);
                if (transaction == null)
                {
                    return NotFound(new { success = false, message = "Transaction not found." });
                }

                // Kiểm tra thông tin ngân hàng
                var bankInfo = _context.Thongtintaikhoan.FirstOrDefault(b => b.StoreId == storeId);
                if (bankInfo == null || string.IsNullOrEmpty(bankInfo.BankAccount))
                {
                    return NotFound(new {  message = "Cửa hàng của bạn chưa thêm tài khoản ngân hàng!" });
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

        [HttpGet("get-banks")]
        public async Task<IActionResult> GetBankList()
        {
            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync("https://api.vietqr.io/v2/banks");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Không thể lấy danh sách ngân hàng từ VietQR." });
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var bankData = JsonConvert.DeserializeObject<Bank>(jsonResponse);

                if (bankData == null || bankData.data == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu ngân hàng không hợp lệ." });
                }

                // Trả về danh sách ngân hàng JSON
                return Ok(bankData.data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("CreateTransaction")]
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
                return BadRequest(new { success = false, message = "Dữ liệu giao dịch bị thiếu." });
            }

            if (newTransaction.Amount <= 0)
            {
                return BadRequest(new { success = false, message = "Số tiền giao dịch không hợp lệ." });
            }

            if (newTransaction.BanId <= 0)
            {
                return BadRequest(new { success = false, message = "Số bàn không hợp lệ." });
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
                var table = await _context.bans.FirstOrDefaultAsync(b => b.BanId == newTransaction.BanId && b.StoreId == newTransaction.StoreId);
                if (table != null)
                {
                    table.IsInUse = "Nocontent"; // Đánh dấu bàn đã có khách
                }
                else
                {
                    return BadRequest(new { success = false, message = "Bàn không tồn tại hoặc không thuộc cửa hàng này." });
                }
                // Lưu vào database
                _context.transactions.Add(newTransaction);
                await _context.SaveChangesAsync();
              
                return Ok(new
                {
                    success = true,
                    message = "Transaction created successfully.",
                    transactionId = newTransaction.TransactionId
                });
            

            }
                catch (Exception ex)
                {
                    // Xử lý lỗi bất ngờ
                    return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
                }
        }


        [HttpPost("save")]
        public async Task<IActionResult> SaveAccount([FromBody] Thongtintaikhoan request)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            try
            {
                if (string.IsNullOrEmpty(request.BankAccount) || request.AcqId <= 0)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });
                }
                request.StoreId = int.Parse(storeId);
                var existingAccount = _context.Thongtintaikhoan.FirstOrDefault(a => a.StoreId == request.StoreId);
                if (existingAccount != null)
                {
                    return BadRequest(new { success = false, message = "Cửa hàng đã có tài khoản ngân hàng!" });
                }
              
                var account = new Thongtintaikhoan
                {
                    BankAccount = request.BankAccount,
                    AccountName = request.AccountName,
                    AcqId = request.AcqId,
                    ShortName = request.ShortName,
                    StoreId = request.StoreId
                };

                _context.Thongtintaikhoan.Add(account);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Lưu thông tin tài khoản thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPut("update/{bankid}")]
        public async Task<IActionResult> UpdateAccount(int bankid, [FromBody] Thongtintaikhoan request)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);
            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            try
            {
                // Kiểm tra đầu vào hợp lệ
                if (string.IsNullOrEmpty(request.BankAccount))
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });
                }

                // Tìm tài khoản trong database
                var existingAccount = await _context.Thongtintaikhoan.FirstOrDefaultAsync(a => a.bankid == bankid && a.StoreId == int.Parse(storeId));

                if (existingAccount == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy tài khoản ngân hàng cần cập nhật." });
                }

                // Cập nhật thông tin tài khoản
                existingAccount.BankAccount = request.BankAccount;
                existingAccount.AccountName = request.AccountName;
                existingAccount.AcqId = request.AcqId;
                existingAccount.ShortName = request.ShortName;

                _context.Thongtintaikhoan.Update(existingAccount);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Cập nhật thông tin tài khoản thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("Getinfobank")]
        public async Task<ActionResult<NhanVien>> Getinfobank()
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var storeid = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeid))
            {
                return Unauthorized();
            }

            // Tìm sản phẩm theo id và StoreId
            var sp = await _context.Thongtintaikhoan.Where(s => s.StoreId == int.Parse(storeid)).ToListAsync();
            Console.WriteLine("thong tin tài khoản:" + sp);
            if (sp == null)
            {
                return NotFound();
            }

            return Ok(sp);
        }
        [HttpGet("GetOrderDetails/{banId}")]
        public async Task<IActionResult> GetOrderDetails(int banId)
        {
            if (banId <= 0)
            {
                return BadRequest(new { success = false, message = "banId không hợp lệ." });
            }

            var orderDetails = await _context.ChiTietDonHangs
                .Where(o => o.BanId == banId)
                .OrderByDescending(o => o.NgayTao)
                .Select(o => new
                {
                    o.CTDHId,
                    o.BanId,
                    o.NgayTao,
                    o.ImageCheckBank, // ✅ Lấy ảnh đã upload
                    o.StoreId, // Nếu cần StoreId để xử lý
                    Products = _context.sanPhamDonHangs
                        .Where(sp => sp.CTDHId == o.CTDHId)
                        .Join(_context.SanPhams,
                              sp => sp.SanPhamId,
                              p => p.SanPhamId,
                              (sp, p) => new
                              {
                                  p.Ten,
                                  sp.SoLuong,
                                  p.ImageBase64, // Ảnh sản phẩm
                                  sp.TongTien
                              }).ToList()
                }).FirstOrDefaultAsync();

            if (orderDetails == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy đơn hàng." });
            }
            Console.WriteLine("Order Details Trước khi trả về: " + JsonConvert.SerializeObject(orderDetails));
            return Ok(orderDetails);
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

