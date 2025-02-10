using Microsoft.AspNetCore.Mvc;
using QLCH_MVC.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Identity.Data;
using System.Text;
namespace QLCH_MVC.Controllers
{
    public class EmployeeController : Controller
    {
         private readonly HttpClient _httpClient;
         
        public EmployeeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7126");
        }
        [HttpGet]
        public  async Task<ActionResult>  Getnhanvien()
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            var token = HttpContext.Session.GetString("JWTToken");
            var request = new HttpRequestMessage(HttpMethod.Get, "api/NhanVien/Getnhanvien");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
           
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var nhanViens = JsonConvert.DeserializeObject<IEnumerable<NhanVien>>(jsonResponse);
                return View(nhanViens);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return View("Error");
            }

        }
        [HttpPost]
        public async Task<IActionResult> Postnhanvien(NhanVien model, IFormFile ImageURL)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = $"Vui lòng nhập đầy đủ thông tin!" });
            }

            // Tạo MultipartFormDataContent
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(model.TenNV), "TenNV");
            formData.Add(new StringContent(model.SDT), "SDT");
            formData.Add(new StringContent(model.NgayVaoLam.ToString("yyyy-MM-dd")), "NgayVaoLam");
            formData.Add(new StringContent(model.MucLuong.ToString()), "MucLuong");

            // Thêm file ảnh vào form data
            if (ImageURL != null && ImageURL.Length > 0)
            {
                var streamContent = new StreamContent(ImageURL.OpenReadStream());
                formData.Add(streamContent, "ImageURL", ImageURL.FileName);
            }
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            // Lấy token từ session và gửi request
            var token = HttpContext.Session.GetString("JWTToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gửi POST request
            var response = await _httpClient.PostAsync("/api/NhanVien/Postnhanvien", formData);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Tạo tài khoản thành công!" });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
            
                return Json(new { success = false, message = $"Đã có lỗi xảy ra:"+ error });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Putnhanvien(int id)
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            var token = HttpContext.Session.GetString("JWTToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"https://localhost:7126/api/NhanVien/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }


            var employeeJson = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<NhanVien>(employeeJson);

            return View(employee);
        }

        [HttpPost]  
        public async Task<IActionResult> PutNhanvien(int id, NhanVien nhanvien, IFormFile ImageURL)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (string.IsNullOrEmpty(nhanvien.TenNV) ||
                string.IsNullOrEmpty(nhanvien.SDT) ||
                nhanvien.NgayVaoLam == default(DateTime) ||
                nhanvien.MucLuong <= 0)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!");
                return View(nhanvien);
            }

            // Tạo formdata để gửi đến API
            var formdata = new MultipartFormDataContent();
            formdata.Add(new StringContent(nhanvien.TenNV ?? string.Empty), "TenNV");
            formdata.Add(new StringContent(nhanvien.SDT ?? string.Empty), "SDT");
            formdata.Add(new StringContent(nhanvien.NgayVaoLam.ToString("yyyy-MM-dd")), "NgayVaoLam");
            formdata.Add(new StringContent(nhanvien.MucLuong.ToString()), "MucLuong");

            // Thêm file ảnh vào formdata nếu có
            if (ImageURL != null && ImageURL.Length > 0)
            {
                var fileStream = new StreamContent(ImageURL.OpenReadStream());
                formdata.Add(fileStream, "ImageURL", ImageURL.FileName);
            }

            // Thêm Authorization Header
            var token = HttpContext.Session.GetString("JWTToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gửi PUT request
            var response = await _httpClient.PutAsync($"https://localhost:7126/api/NhanVien/{id}", formdata);

            // Xử lý phản hồi
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Getnhanvien");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Lỗi từ API: {error}");
                return View(nhanvien);
            }
        }
        [HttpDelete]         
        public async Task<IActionResult> Deletenhanvien(int id)
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            var token = HttpContext.Session.GetString("JWTToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var reponse = await _httpClient.DeleteAsync($"https://localhost:7126/api/NhanVien/{id}");
        
                if (!reponse.IsSuccessStatusCode)
                {
                    var errorMessage = await reponse.Content.ReadAsStringAsync();
                    return BadRequest($"Không thể xóa nhân viên. Chi tiết lỗi: {errorMessage}");
                }

            return Ok();
        }



    }
}
