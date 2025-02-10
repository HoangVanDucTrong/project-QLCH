using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using QLCH_MVC.Models;
using System.Net.Http.Headers;
using QLCH_MVC.Models.LoginDTO;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using QLCH_MVC.Models.resetpassword;
using System.Reflection;
using System;
namespace QLCH_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
           _httpClient.BaseAddress = new Uri("https://localhost:7126");
        }
        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }
        public IActionResult ConfirmEmailNotice()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDTO model)
        {
            if (!ModelState.IsValid)
            {

                // Kiểm tra và ghi log lỗi ModelState
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }

                // Thêm thông báo lỗi cho người dùng
                ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ thông tin.");
                return View(model);

            }

            var registerRequest = new
            {
                Username = model.Username,
                Sdt = model.Sdt,
                DiaChi = model.DiaChi,
                QuocGia = model.QuocGia,
                Password = model.Password,
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");
           Console.WriteLine($"Request Content: {await jsonContent.ReadAsStringAsync()}");
            var response = await _httpClient.PostAsync("/api/Store/Register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return View("ConfirmEmailNotice");
            }

            // Lấy nội dung phản hồi từ API để biết thêm chi tiết về lỗi
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Error Response: {errorMessage}");
            ModelState.AddModelError(string.Empty, $"Registration failed. {errorMessage}");
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
           

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string role)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ thông tin.");
                return View(model);
            }

            var loginRequest = new
            {
                Username = model.Username,
                Password = model.Password
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

            string apiUrl = string.Empty;
            Console.WriteLine("quyenèn trả về là:" + role);
            // Xác định API cần gọi dựa trên role (admin hoặc staff)
            if (role == "admin")
            {
                apiUrl = "/api/Store/Login"; // API cho admin
            }
            else if (role == "staff")
            {
                apiUrl = "/api/AccountStaff/EmployeeLogin"; // API cho nhân viên
            }

            if (string.IsNullOrEmpty(apiUrl))
            {
                ViewBag.ErrorMessage = "Role không hợp lệ!";
                return View(model);
            }

            var response = await _httpClient.PostAsync(apiUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                HttpContext.Session.SetString("UserRoles", string.Join(",", loginResponse.Roles)); // Lưu roles vào session

                // Lưu JWT token vào session
                HttpContext.Session.SetString("JWTToken", loginResponse.JwtToken);

                return RedirectToAction("Index", "Home");
            }

            // Xử lý khi đăng nhập không thành công
            var errorResponseBody = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = "Username or password incorrect";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var response = await _httpClient.PostAsync("https://localhost:7126/api/Store/Logout", null);

                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.Remove("JWTToken");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    // Handle error if needed
                    ModelState.AddModelError(string.Empty, "Logout failed.");
                    return View("Error");
                }
            }
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
      
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng điền email của bạn!!");
                return View(forgotPasswordModel);
            }
            var resetpass = new
            {
                Email = forgotPasswordModel.Email,
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(resetpass), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/Store/ForgotPassword", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ResetPasswordWithCode", "Account");
            }
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Send Code Failed. {errorMessage}");
            return View(forgotPasswordModel);


        }
        

        [HttpGet]   
        public IActionResult ResetPasswordWithCode()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordWithCode( ResetPasswordRequestDTO resetPasswordRequestDTO)
        {
            if (!ModelState.IsValid)  // Đúng: kiểm tra nếu ModelState không hợp lệ
            {
                ModelState.AddModelError(string.Empty, "Vui lòng điền đầy đủ thông tin!!");
                return View(resetPasswordRequestDTO);
            }
            var ContentReset = new
            {
                Email = resetPasswordRequestDTO.Email,
                Token = resetPasswordRequestDTO.Token,
                NewPassword = resetPasswordRequestDTO.NewPassword
            };
            var jsonContent = new StringContent(JsonSerializer.Serialize(ContentReset), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Store/ResetPasswordWithCode", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Account");
            }
            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"RESET FAILED! {errorMessage}");
            return View(resetPasswordRequestDTO);
        }
    }
}
