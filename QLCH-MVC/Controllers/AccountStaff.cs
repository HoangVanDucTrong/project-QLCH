using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; 
using System.Text.Json;
using System.Net.Http.Headers;

using QLCH_MVC.Models;
using System.Reflection;
using System.Text;
namespace QLCH_MVC.Controllers
{
    public class AccountStaff : Controller
    {
        private readonly HttpClient _httpClient;
        public AccountStaff(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7126");
        }
        [HttpGet]
        public async Task<ActionResult> AccountNv()
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;

            var token = HttpContext.Session.GetString("JWTToken");

            // Gửi yêu cầu lấy danh sách nhân viên từ API
            var request = new HttpRequestMessage(HttpMethod.Get, "api/NhanVien/Getnhanvien");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) // ✅ Xử lý riêng lỗi 404
            {
                ViewBag.Message = "Không có nhân viên nào!";
                return View(new List<NhanVien>()); // Trả về danh sách rỗng thay vì trang lỗi
            }
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return View("Error");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var nhanViens = JsonConvert.DeserializeObject<List<NhanVien>>(jsonResponse);

            if (nhanViens == null || !nhanViens.Any())
            {
                ViewBag.Message = "Không có nhân viên nào!";
                return View(new List<NhanVien>());
            }

            // ✅ Tạo danh sách các tác vụ kiểm tra tài khoản nhân viên (chạy song song)
            var tasks = nhanViens.Select(async nhanVien =>
            {
                var checkAccountRequest = new HttpRequestMessage(HttpMethod.Get, $"api/AccountStaff/checkAccountnv?NVid={nhanVien.NVid}");
                checkAccountRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var checkAccountResponse = await _httpClient.SendAsync(checkAccountRequest);

                if (checkAccountResponse.IsSuccessStatusCode)
                {
                    var jsonStatusResponse = await checkAccountResponse.Content.ReadAsStringAsync();
                    var statusResponse = JsonConvert.DeserializeObject<dynamic>(jsonStatusResponse);
                    nhanVien.AccountStatus = statusResponse?.status.ToString(); // ✅ Lưu trạng thái tài khoản
                }
                else if (checkAccountResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    nhanVien.AccountStatus = "Chưa có tài khoản"; // ✅ Nếu 404 thì ghi rõ
                }
                else
                {
                    nhanVien.AccountStatus = "Lỗi khi kiểm tra"; // ✅ Các lỗi khác
                }
            });

            await Task.WhenAll(tasks); // ✅ Đợi tất cả request hoàn thành

            return View(nhanViens); // Trả về View với danh sách nhân viên và trạng thái tài khoản
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAccount(TaiKhoanNhanVien model)
        {
            // Kiểm tra tính hợp lệ của Model
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = $"Vui lòng nhập thông tin hợp lệ!" });
            }

            // Lấy token từ session
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy token. Vui lòng đăng nhập lại.");
                return View("AccountNv", model);
            }

            // Tạo yêu cầu gửi đến API
            var registerRequest = new
            {
                TenDangNhap = model.TenDangNhap,
                MatKhau = model.MatKhau,
                NVid = model.NVid,
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7126/api/AccountStaff/CreateEmployeeAccount");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = jsonContent;

            // Gửi yêu cầu
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Tạo tài khoản thành công!" });
            }

            // Lấy lỗi từ API nếu thất bại
            var errorMessage = await response.Content.ReadAsStringAsync();
            return Json(new { success = false, message = $"Tạo tài khoản thất bại: {errorMessage}" });
        }


        
    }
}
