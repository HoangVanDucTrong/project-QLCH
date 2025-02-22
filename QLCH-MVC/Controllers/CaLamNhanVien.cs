using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using QLCH_MVC.Models;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
namespace QLCH_MVC.Controllers
{
    public class CaLamNhanVien : Controller
    {
        private readonly HttpClient _httpClient;

        public CaLamNhanVien(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7126");
        }

        [HttpGet]
        public async Task<ActionResult> GetCaLam()
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            var token = HttpContext.Session.GetString("JWTToken");
            var request = new HttpRequestMessage(HttpMethod.Get, "api/CaLamNV/GetCaLam");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var calam = JsonConvert.DeserializeObject<IEnumerable<Calam>>(jsonResponse);

                // Kiểm tra nếu không có dữ liệu
                if (calam == null || !calam.Any())
                {
                    ViewBag.Message = "Không có dữ liệu nào trong tuần này.";
                    return View(new List<Calam>()); // Trả về danh sách trống
                }

                return View(calam);
            }
            else
            {
                // Ghi log lỗi và trả về View kèm thông báo lỗi
                Console.WriteLine($"Error: {response.StatusCode}");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi tải dữ liệu. Vui lòng thử lại sau.";
                return View(new List<Calam>()); // Trả về danh sách trống
            }
        }

        [HttpGet]
        public async Task<IActionResult> Filter(DateTime startDate, DateTime endDate)
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            // Thêm token vào header Authorization
            var token = HttpContext.Session.GetString("JWTToken"); // Giả sử token được lưu trong Session
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API
            var response = await _httpClient.GetAsync($"https://localhost:7126/api/CaLamNV/Filter?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Kiểm tra phản hồi
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewBag.Error = "Không có dữ liệu nào trong khoảng thời gian này.";
                return View("GetCaLam", new List<Calam>()); // Trả về danh sách rỗng và hiển thị View
            }
            // Đọc dữ liệu từ API
            var jsonData = await response.Content.ReadAsStringAsync();
            var filteredData = JsonConvert.DeserializeObject<List<Calam>>(jsonData);      
            return View("GetCaLam", filteredData);
        }
        [HttpGet]
        public async Task<IActionResult>  Postcalam()
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
        public async Task<IActionResult> PostCaLam([FromBody]  List<CalamViewModel> caLamList)
        { 
            if (caLamList == null || !caLamList.Any())
            {
                return BadRequest("Vui lòng cung cấp danh sách ca làm.");
            }

         

            // Mapping từ ViewModel sang Model trước khi gọi API nội bộ
            var mappedCalamList = caLamList.Select(ca => new Calam
            {
                NVid = ca.NVid,
                NgayLam = ca.NgayLam,
                GioBatDau = ca.GioBatDau,
                GioKetThuc = ca.GioKetThuc,
                GhiChu = ca.GhiChu,
                calam = ca.calam,
            }).ToList();

            var token = HttpContext.Session.GetString("JWTToken"); // Lấy token từ Session
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng đến trang Login nếu chưa có token
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("https://localhost:7126/api/CaLamNV", mappedCalamList);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetCaLam"); // Chuyển hướng đến danh sách ca làm sau khi thành công
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error: {errorMessage}");
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Putcalam(DateTime startDate, DateTime endDate)
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

            try
            {
                // Gửi request lấy danh sách nhân viên
                var responseNhanVien = await _httpClient.GetAsync("https://localhost:7126/api/NhanVien/Getnhanvien");

                // Gửi request lọc danh sách ca làm theo khoảng thời gian
                var responseCaLam = await _httpClient.GetAsync($"https://localhost:7126/api/CaLamNV/Filter?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

                // Kiểm tra trạng thái của cả hai phản hồi
                if (responseNhanVien.IsSuccessStatusCode && responseCaLam.IsSuccessStatusCode)
                {
                    var nhanVienJson = await responseNhanVien.Content.ReadAsStringAsync();
                    var caLamJson = await responseCaLam.Content.ReadAsStringAsync();

                    // Deserialize dữ liệu
                    var nhanViens = JsonConvert.DeserializeObject<IEnumerable<NhanVien>>(nhanVienJson);
                    var caLams = JsonConvert.DeserializeObject<IEnumerable<Calam>>(caLamJson);

                    // Gói dữ liệu vào ViewModel
                    var viewModel = new PutCalamViewModel
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        NhanViens = nhanViens,
                        CaLams = caLams
                    };


                    return View(viewModel);
                }
                else
                {
                    // Xử lý lỗi phản hồi
                    var errorMessage = $"Error fetching data. NhanVien: {responseNhanVien.StatusCode}, CaLam: {responseCaLam.StatusCode}";
                    Console.WriteLine(errorMessage);
                    ViewBag.Error = errorMessage;
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi ngoại lệ
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return View("Error");
            }
        }
        /*
        [HttpPut]
        public async Task<IActionResult> PutCaLam([FromBody] List<CalamViewModel> caLamList)
        {
            if (caLamList == null || !caLamList.Any())
            {
                Console.WriteLine("Dữ liệu nhận được từ client: null hoặc trống.");
                return BadRequest("Vui lòng cung cấp danh sách ca làm.");
            }

            Console.WriteLine($"Dữ liệu nhận được từ client: {JsonConvert.SerializeObject(caLamList)}");

            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.PutAsJsonAsync("https://localhost:7126/api/CaLamNV", caLamList);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetCaLam");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error from API: {errorMessage}");
                    ModelState.AddModelError(string.Empty, $"Error: {errorMessage}");
                    return View("PutCaLam", caLamList); // Trả về View hiện tại với thông báo lỗi
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi yêu cầu đến API: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
                return View("PutCaLam", caLamList);
            }
        }*/

        [HttpGet]
        public async Task<IActionResult> DeleteCalam(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này." });
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.DeleteAsync($"https://localhost:7126/api/CaLamNV/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Xóa ca làm thành công." });
                }
                else
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa ca làm." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }


    }
}
