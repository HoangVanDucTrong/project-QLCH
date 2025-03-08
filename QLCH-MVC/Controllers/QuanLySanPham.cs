using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLCH_MVC.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace QLCH_MVC.Controllers
{
   // [Route("QuanLySanPham")]
    public class QuanLySanPham : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public QuanLySanPham(IHttpClientFactory httpClientFactory, HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("https://localhost:7126");
        }
        private void AddAuthorizationHeader(HttpClient client)
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        [HttpGet]
        public async Task<ActionResult> DanhSachSanPham()
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            var token = HttpContext.Session.GetString("JWTToken");
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Sanpham/Getsanpham");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var nhanViens = JsonConvert.DeserializeObject<IEnumerable<SanPham>>(jsonResponse);
                return View(nhanViens);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostSanPham([FromForm] SanPham model, [FromForm] IFormFile ImageURL)

        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Lỗi dữ liệu: " + string.Join(", ", errors) });
            }

            try
            {
                // Tạo MultipartFormDataContent
                var formData = new MultipartFormDataContent
        {
            { new StringContent(model.Ten), "Ten" },
            { new StringContent(model.Gia.ToString()), "Gia" },
            { new StringContent(model.MoTa), "MoTa" },
            { new StringContent(model.DanhMucId.ToString()), "DanhMucId" },
            { new StringContent(model.StoreId?.ToString() ?? "0"), "StoreId" }
        };

                // Xử lý file ảnh
                if (ImageURL != null && ImageURL.Length > 0)
                {
                    var streamContent = new StreamContent(ImageURL.OpenReadStream());
                    formData.Add(streamContent, "ImageURL", ImageURL.FileName);
                }
                var userRoles = HttpContext.Session.GetString("");
                var roles = userRoles?.Split(',') ?? new string[0];
                ViewBag.UserRoles = roles;
                // Lấy token từ session
                var token = HttpContext.Session.GetString("JWTToken") ?? Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại!" });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Gửi request tới API
                var response = await _httpClient.PostAsync("/api/Sanpham", formData);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Thêm Sản Phẩm thành công!" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"Lỗi từ API: {error}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSanPham(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:7126/api/Sanpham/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return NotFound(new { success = false, message = $"Không tìm thấy sản phẩm! Lỗi: {errorMessage}" });
            }
            
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<SanPham>(jsonResponse);

            return Json(new { success = true, data = product });
        }



        [HttpPut]
        public async Task<IActionResult> PutSanPham(int id, [FromForm] SanPham model, IFormFile ImageURL)
        {
            try
            {
                var formData = new MultipartFormDataContent
        {
            { new StringContent(model.Ten), "Ten" },
            { new StringContent(model.Gia.ToString()), "Gia" },
            { new StringContent(model.MoTa), "MoTa" },
            { new StringContent(model.DanhMucId.ToString()), "DanhMucId" }
        };

                // Nếu có ảnh mới thì thêm vào formData
                if (ImageURL != null && ImageURL.Length > 0)
                {
                    var streamContent = new StreamContent(ImageURL.OpenReadStream());
                    formData.Add(streamContent, "ImageURL", ImageURL.FileName);
                }
                else
                {
                    // Nếu không có ảnh mới, lấy ảnh cũ từ request
                    var oldImageBase64 = HttpContext.Request.Form["ImageURL"];
                    if (!string.IsNullOrEmpty(oldImageBase64))
                    {
                        formData.Add(new StringContent(oldImageBase64), "ImageURL"); // Gửi ảnh cũ
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "❌ Lỗi: Hình ảnh không được để trống!" });
                    }
                }

                // Lấy token từ session
                var token = HttpContext.Session.GetString("JWTToken") ?? Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại!" });
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Gửi request PUT
                var response = await _httpClient.PutAsync($"https://localhost:7126/api/Sanpham/{id}", formData);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "✅ Sửa Sản Phẩm thành công!" });
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = $"❌ Lỗi từ API: {error}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpDelete]
        public async Task<IActionResult> DeleteSanPham(int id)
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            var token = HttpContext.Session.GetString("JWTToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var reponse = await _httpClient.DeleteAsync($"https://localhost:7126/api/Sanpham/{id}");

            if (!reponse.IsSuccessStatusCode)
            {
                var errorMessage = await reponse.Content.ReadAsStringAsync();
                return BadRequest($"Không thể xóa nhân viên. Chi tiết lỗi: {errorMessage}");
            }

            return Ok();
        }
    }
}
