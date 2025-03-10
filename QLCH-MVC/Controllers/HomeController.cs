
﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLCH_MVC.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace QLCH_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7126");
        }

        public async Task<IActionResult> Index()
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
            var sanPhamRequest = new HttpRequestMessage(HttpMethod.Get, "api/Sanpham/Getsanpham");
            sanPhamRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var sanPhamResponse = await _httpClient.SendAsync(sanPhamRequest);

            // Gửi yêu cầu lấy danh sách bàn
            var banRequest = new HttpRequestMessage(HttpMethod.Get, "api/Ban/Getban");
            banRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var banResponse = await _httpClient.SendAsync(banRequest);

            var sanPhams = new List<SanPham>();
            var bans = new List<Ban>();

            bool hasBan = false;
            bool hasSanPham = false;

            // Kiểm tra dữ liệu bàn
            if (banResponse.IsSuccessStatusCode)
            {
                var banJson = await banResponse.Content.ReadAsStringAsync();
                bans = JsonConvert.DeserializeObject<List<Ban>>(banJson);
                hasBan = bans.Any();
            }

            // Kiểm tra dữ liệu sản phẩm
            if (sanPhamResponse.IsSuccessStatusCode)
            {
                var sanPhamJson = await sanPhamResponse.Content.ReadAsStringAsync();
                sanPhams = JsonConvert.DeserializeObject<List<SanPham>>(sanPhamJson);
                hasSanPham = sanPhams.Any();
            }

            var viewModel = new SanPhamBanViewModel
            {
                SanPhams = sanPhams,
                Bans = bans
            };

            // Xử lý lỗi từng loại
            if (!hasBan)
            {
                ViewBag.BanError = "Cửa hàng của bạn chưa có bàn! Hãy thêm ngay!";
            }
            if (!hasSanPham)
            {
                ViewBag.SanPhamError = "Cửa hàng của bạn chưa có sản phẩm! Hãy thêm ngay!";
            }

            return View(viewModel);
        }


        public async Task<IActionResult> Getsanpham()                                           
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            var token = HttpContext.Session.GetString("JWTToken");
            ViewBag.JWTToken = token;
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gửi yêu cầu lấy danh sách sản phẩm
            var sanPhamRequest = new HttpRequestMessage(HttpMethod.Get, "api/Sanpham/Getsanpham");
            sanPhamRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var sanPhamResponse = await _httpClient.SendAsync(sanPhamRequest);
            if(sanPhamResponse.IsSuccessStatusCode)
            {
                var sanPhamJson = await sanPhamResponse.Content.ReadAsStringAsync();
                var sanPhams = JsonConvert.DeserializeObject<IEnumerable<SanPham>>(sanPhamJson);
                return Json(sanPhams);
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> SearchSanPham(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new List<SanPham>()); // Trả về danh sách rỗng nếu query rỗng
            }

            var token = HttpContext.Session.GetString("JWTToken"); // Lấy JWT token từ Session
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing or invalid."); // Trả về lỗi 401 nếu token không hợp lệ
            }

            try
            {
                // Gửi yêu cầu đến API
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"https://localhost:7126/api/Sanpham/api/sanpham/search?query={query}");
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var sanPhams = JsonConvert.DeserializeObject<List<SanPham>>(responseData);
                  

                    // Trả về JSON cho JavaScript
                    return Json(sanPhams);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "API call failed.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] transaction transaction)
        {
            // Lấy JWT token từ Session
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { success = false, message = "Token is missing. Please log in again." });
            }

            if (transaction == null || transaction.Amount <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid transaction data." });
            }

            try
            {
                // URL API cần gọi
                var apiUrl = "https://localhost:7126/api/Thanhtoan/CreateTransaction";

                // Chuẩn bị dữ liệu JSON
                var jsonContent = JsonConvert.SerializeObject(transaction);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Tạo HttpClient
                using (var client = new HttpClient())
                {
                    // Gửi kèm header Authorization với token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Gửi request đến API
                    var response = await client.PostAsync(apiUrl, httpContent);

                    // Xử lý kết quả
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        var transactionId = result.transactionId;

                        // Trả về dữ liệu bao gồm transactionId
                        var jsonResult = Json(new { success = true, message = "Transaction created ô sờ kê .", transactionId = transactionId.ToString() });
                        return jsonResult;

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
        public async Task<IActionResult> GenerateQrView(int transactionId)
        {
            try
            {
                // Chuẩn bị dữ liệu gửi đến API
                var apiUrl = "https://localhost:7126/api/Thanhtoan/stores/createqr";
                var transaction = new
                {
                    TransactionId = transactionId,
                };
                var jsonContent = JsonConvert.SerializeObject(transaction);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Lấy token từ session
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token is missing. Please log in again.");
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Gửi yêu cầu đến API
                    var response = await client.PostAsync(apiUrl, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                       
                        var apiResult = JsonConvert.DeserializeObject<dynamic>(responseContent);

                        // Lấy URL mã QR từ API
                        string qrCodeUrl = apiResult?.qrCodeUrl;
                       
                        // Trả về view với mã QR
                        return PartialView("_QrCodeView", model: qrCodeUrl);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return BadRequest($"Đã có lỗi xảy ra: {errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
   
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
