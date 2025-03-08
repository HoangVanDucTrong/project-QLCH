using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLCH_MVC.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace QLCH_MVC.Controllers
{
    public class BanController : Controller
    {
            private readonly ILogger<HomeController> _logger;
            private readonly HttpClient _httpClient;

            public BanController(ILogger<HomeController> logger, HttpClient httpClient)
            {
                _logger = logger;
                _httpClient = httpClient;
                _httpClient.BaseAddress = new Uri("https://localhost:7126");
            }
        public async Task<IActionResult> IndexBan()
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;

            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gửi yêu cầu lấy danh sách sản phẩm


            // Gửi yêu cầu lấy danh sách bàn
            var banRequest = new HttpRequestMessage(HttpMethod.Get, "api/Ban/Getban");
            banRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var banResponse = await _httpClient.SendAsync(banRequest);
            var bans = new List<Ban>();

            bool hasBan = false;
            if (banResponse.IsSuccessStatusCode)
            {
                var banJson = await banResponse.Content.ReadAsStringAsync();
                bans = JsonConvert.DeserializeObject<List<Ban>>(banJson);
                hasBan = bans.Any();
            }
            var viewModel = new BanViewModel
            {
                Bans = bans

            };
            if (!hasBan)
            {
                ViewBag.BanError = "Cửa hàng của bạn chưa có bàn! Hãy thêm ngay!";
            }
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBan(Ban ban)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (ban.SoBan <= 0)
            {
                TempData["ErrorMessage"] = "Số bàn không hợp lệ!";
                return RedirectToAction("IndexBan");
            }


            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(ban.SoBan.ToString()), "SoBan");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync("api/Ban/PostBan", formData);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            try
            {
                var result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                string message = result.message?.ToString() ?? "Có lỗi xảy ra!";

                if (response.IsSuccessStatusCode && result.success == true)
                {
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            catch
            {

                return Json(new { success = false, message = "Lỗi hệ thống! Không thể xử lý phản hồi từ API." });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetQR(int banId)
        {
            Console.WriteLine("ID nhận được: " + banId);

            if (banId <= 0)
            {
                Console.WriteLine("Lỗi: banId không hợp lệ");
                return BadRequest("ID bàn không hợp lệ");
            }

            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Lỗi: JWTToken không tồn tại");
                    return Unauthorized("Bạn chưa đăng nhập!");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Gửi yêu cầu lấy QR từ API backend
                var response = await _httpClient.GetAsync($"https://localhost:7126/api/QR/CreateQR?banId={banId}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Lỗi từ API backend: {errorMsg}");
                    return BadRequest("Không thể lấy mã QR!");
                }

                var qrImageBytes = await response.Content.ReadAsByteArrayAsync();
                return File(qrImageBytes, "image/png");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy mã QR: " + ex.Message);
                return StatusCode(500, "Lỗi hệ thống khi lấy mã QR!");
            }
        }
        [HttpGet]
        public async Task<IActionResult> ShowProducts(int banId, int storeId)
        {
            // Gửi request trực tiếp mà không cần token
            var response = await _httpClient.GetAsync($"api/QR/store/products?banId={banId}&storeId={storeId}");

            var products = new List<SanPham>();

            bool hasProducts = false;

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<SanPham>>(jsonResponse);
                hasProducts = products.Any();
            }

            var viewModel = new SanPhamViewModel
            {
                sanPhams = products
            };

            if (!hasProducts)
            {
                ViewBag.BanError = "Cửa hàng này chưa có sản phẩm!";
            }
            ViewBag.banid = banId;
            ViewBag.StoreId = storeId;
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] transaction transaction)
        {
            if (transaction == null || transaction.Amount <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid transaction data." });
            }

            try
            {
                // URL API cần gọi
                var apiUrl = "https://localhost:7126/api/ThanhtoanClient/CreateTransaction";

                // Chuẩn bị dữ liệu JSON
                var jsonContent = JsonConvert.SerializeObject(transaction);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Tạo HttpClient
                using (var client = new HttpClient())
                {
                    // Không cần gửi token ở đây

                    // Gửi request đến API
                    var response = await client.PostAsync(apiUrl, httpContent);

                    // Xử lý kết quả
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        var transactionId = result?.transactionId;
             
                        // Trả về dữ liệu bao gồm transactionId
                        return Json(new { success = true, message = "Transaction created successfully.", transactionId = transactionId?.ToString() });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new { success = false, message = $"Đã có lỗi xảy ra: {errorContent}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GenerateQrView(int transactionId,int storeId)
        {
            try
            {
                // Chuẩn bị dữ liệu gửi đến API
                var apiUrl = "https://localhost:7126/api/ThanhtoanClient/CreateQrForStore";
                var transaction = new
                {
                    TransactionId = transactionId,
                    StoreId = storeId,
                };

                var jsonContent = JsonConvert.SerializeObject(transaction);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    // Không gửi token trong request này
                    // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Gửi yêu cầu đến API
                    var response = await client.PostAsync(apiUrl, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                  
                        var apiResult = JsonConvert.DeserializeObject<dynamic>(responseContent);

                        // Lấy URL mã QR từ API
                        string qrCodeUrl = apiResult?.qrCodeUrl;

                        if (string.IsNullOrEmpty(qrCodeUrl))
                        {
                            return BadRequest("Không nhận được URL mã QR từ API.");
                        }

                        // Trả về partial view hiển thị mã QR
                        return PartialView("_QRCodeClient", model: qrCodeUrl);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return BadRequest($"Đã có lỗi xảy ra từ API: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTableStatus(int BanId, int Storeid)
        {
            Console.WriteLine($"banid: {BanId}");
            Console.WriteLine($"storeid: {Storeid}");

            using (var client = new HttpClient())
            {
                var apiUrl = "https://localhost:7126/api/ThanhtoanClient/statusBan";
                var requestBody = new
                {
                    banid = BanId,
                    storeid = Storeid
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, jsonContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Trạng thái bàn đã được cập nhật." });
                }
                else
                {
                    return Json(new { success = false, message = $"Lỗi API: {responseContent}" });
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            if (orderRequest == null || orderRequest.Products == null || orderRequest.Products.Count == 0)
            {
                return Json(new { success = false, message = "Dữ liệu đơn hàng không hợp lệ." });
            }

            try
            {
                var apiUrl = "https://localhost:7126/api/ThanhtoanClient/CreateOrder"; // Đường dẫn API backend
                var jsonContent = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");


                var response = await _httpClient.PostAsync(apiUrl, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Đơn hàng đã gửi thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = $"Lỗi API: {responseString}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi server: {ex.Message}" });
            }
        }


        public async Task<IActionResult> GetOrderDetails(int banId)
        {
         
            // Kiểm tra banId hợp lệ
            if (banId <= 0)
            {
                return BadRequest("banId không hợp lệ.");
            }

            // Lấy token từ session
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // API URL
            var apiUrl = $"https://localhost:7126/api/Thanhtoan/GetOrderDetails/{banId}";

            try
            {
                // Thêm Authorization header với token vào HttpClient
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Gọi API
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
          

                    var apiResult = JsonConvert.DeserializeObject<dynamic>(responseData);
                    var orderDetailsJson = apiResult.orderDetails != null ? apiResult.orderDetails.ToString() : responseData;
                    var orderDetails = JsonConvert.DeserializeObject<OrderDetails>(orderDetailsJson);
                    Console.WriteLine($"API Response Data: {orderDetails}");

                    return Json(orderDetails);  // Trả về đúng kiểu dữ liệu
                }
                else
                {
                    // Nếu API trả về lỗi
                    return NotFound("Không tìm thấy đơn hàng.");
                }
            }
            catch (Exception ex)
            {
                // Lỗi khi gọi API
                return StatusCode(500, "Lỗi khi gọi API: " + ex.Message);
            }
        }

    }


}
