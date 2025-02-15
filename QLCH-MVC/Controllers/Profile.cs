using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QLCH.Models;
using QLCH_MVC.Models;
using System.Net.Http.Headers;
using System.Text;

namespace QLCH_MVC.Controllers
{
    public class Profile : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public Profile(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7126");
        }
        public async Task<IActionResult> profile()
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

            // Gửi yêu cầu lấy thông tin cửa hàng
            var infoStoreRequest = new HttpRequestMessage(HttpMethod.Get, "api/Store/GetinfoStore");
            infoStoreRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var infoStoreResponse = await _httpClient.SendAsync(infoStoreRequest);

            // Gửi yêu cầu lấy thông tin ngân hàng
            var bankRequest = new HttpRequestMessage(HttpMethod.Get, "api/Thanhtoan/Getinfobank");
            bankRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var bankResponse = await _httpClient.SendAsync(bankRequest);

            IEnumerable<Thongtinnganhang> banks = null;
            bool bankNotFound = false;

            if (bankResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                bankNotFound = true; // Đánh dấu là không có tài khoản ngân hàng
            }
            else if (bankResponse.IsSuccessStatusCode)
            {
                var bankJson = await bankResponse.Content.ReadAsStringAsync();
                if (bankJson.Trim().StartsWith("{")) // ✅ Kiểm tra xem API trả về Object hay Array
                {
                    var bank = JsonConvert.DeserializeObject<Thongtinnganhang>(bankJson);
                    banks = new List<Thongtinnganhang> { bank }; // ✅ Chuyển thành danh sách
                }
                else
                {
                    banks = JsonConvert.DeserializeObject<IEnumerable<Thongtinnganhang>>(bankJson);
                }
            }

            if (infoStoreResponse.IsSuccessStatusCode)
            {
                var storejson = await infoStoreResponse.Content.ReadAsStringAsync();
                var storethongtin = JsonConvert.DeserializeObject<store>(storejson);
                var storesList = new List<store> { storethongtin };
                var viewModel = new ThongtinCuahang
                {
                    Stores = storesList,
                    thongtinnganhangs = banks
                };

                ViewBag.BankNotFound = bankNotFound; // ✅ Truyền trạng thái vào ViewBag

                return View(viewModel);
            }
            else
            {
                return View("Error");
            }
        }

        public async Task<IActionResult> AddAccountBank()
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
                var bankRequest = new HttpRequestMessage(HttpMethod.Get, "api/Thanhtoan/get-banks");
                var bankResponse = await _httpClient.SendAsync(bankRequest);

                if (bankResponse.IsSuccessStatusCode)
                {
                    var bankJson = await bankResponse.Content.ReadAsStringAsync();
                    var banks = JsonConvert.DeserializeObject<List<Bank>>(bankJson);

                    // Truyền danh sách ngân hàng xuống View
                    ViewBag.Banks = banks;
               
            }

                return View(new ThongTinTaiKhoanViewModel());
            }
        private async Task<List<Bank>> GetBankListFromVietQR()
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            var token = HttpContext.Session.GetString("JWTToken");
            ViewBag.JWTToken = token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var bankRequest = new HttpRequestMessage(HttpMethod.Get, "api/Thanhtoan/get-banks");
            var bankResponse = await _httpClient.SendAsync(bankRequest);

            if (bankResponse.IsSuccessStatusCode)
            {
                var bankJson = await bankResponse.Content.ReadAsStringAsync();
                var banks = JsonConvert.DeserializeObject<List<Bank>>(bankJson);

                ViewBag.Banks = banks;
                return JsonConvert.DeserializeObject<List<Bank>>(bankJson);
              
            }

            return new List<Bank>(); // Trả về danh sách rỗng nếu có lỗi
        }

        [HttpPost]
        public async Task<IActionResult> SaveAccount(ThongTinTaiKhoanViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (!ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var bankRequest = new HttpRequestMessage(HttpMethod.Get, "api/Thanhtoan/get-banks");
                var bankResponse = await _httpClient.SendAsync(bankRequest);

                if (bankResponse.IsSuccessStatusCode)
                {
                    var bankJson = await bankResponse.Content.ReadAsStringAsync();
                    var banks = JsonConvert.DeserializeObject<List<Bank>>(bankJson);

                   
                    ViewBag.Banks = banks;
        
                }

                return View("AddAccountBank", model);
            }
            var bankList = await GetBankListFromVietQR(); // Hàm này lấy danh sách ngân hàng từ VietQR API
            var selectedBank = bankList.FirstOrDefault(b => b.bin == model.AcqId.ToString());
            if (selectedBank == null)
            {
                ModelState.AddModelError("", "Ngân hàng không tồn tại.");
          
                return View("AddAccountBank", model);
            }
            try
            {
                var requestData = new
                {
                    model.AcqId,
                    model.AccountName,
                    model.BankAccount,
                    ShortName = selectedBank.shortName
                };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                Console.WriteLine("Dữ liệu gửi lên API: " + jsonContent); // ✅ Debug dữ liệu
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");


                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = await client.PostAsync("https://localhost:7126/api/Thanhtoan/save", httpContent);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Phản hồi từ API: " + responseContent);
                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Lưu thông tin tài khoản thành công!";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        string errorMessage = apiResponse.message.ToString();

                        if (errorMessage.Contains("Cửa hàng đã có tài khoản ngân hàng"))
                        {
                            TempData["ErrorMessage"] = errorMessage;
                            return RedirectToAction("AddAccountBank");
                        }

                        ModelState.AddModelError("", "Lỗi API: " + errorMessage);
                        return View("AddAccountBank", model);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                return View("AddAccountBank", model);
            }
        }
        public async Task<IActionResult> EditAccountBank()
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
            var bankRequest = new HttpRequestMessage(HttpMethod.Get, "api/Thanhtoan/get-banks");
            var bankResponse = await _httpClient.SendAsync(bankRequest);

            if (bankResponse.IsSuccessStatusCode)
            {
                var bankJson = await bankResponse.Content.ReadAsStringAsync();
                var banks = JsonConvert.DeserializeObject<List<Bank>>(bankJson);

                // Truyền danh sách ngân hàng xuống View
                ViewBag.Banks = banks;

            }

            return View(new ThongTinTaiKhoanViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAccount(ThongTinTaiKhoanViewModel model)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var bankList = await GetBankListFromVietQR(); // Hàm này lấy danh sách ngân hàng từ VietQR API
            var selectedBank = bankList.FirstOrDefault(b => b.bin == model.AcqId.ToString());
            if (selectedBank == null)
            {
                ModelState.AddModelError("", "Ngân hàng không tồn tại.");

                return View("AddAccountBank", model);
            }
            try
            {
                var jsonContent = JsonConvert.SerializeObject(new
                {
                    BankAccount = model.BankAccount,
                    AccountName = model.AccountName,
                    AcqId = model.AcqId,
                    ShortName = selectedBank.shortName
                });
                // 🔹 Gửi request GET đến API để lấy thông tin ngân hàng
                var bankRequest = new HttpRequestMessage(HttpMethod.Get, "api/Thanhtoan/Getinfobank");
                bankRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var bankResponse = await _httpClient.SendAsync(bankRequest);

                Thongtinnganhang bankInfo = null;

                if (bankResponse.IsSuccessStatusCode)
                {
                    var bankJson = await bankResponse.Content.ReadAsStringAsync();
                    Console.WriteLine("Dữ liệu ngân hàng API trả về: " + bankJson);

                    if (bankJson.Trim().StartsWith("{")) // ✅ Kiểm tra xem API trả về Object hay Array
                    {
                        bankInfo = JsonConvert.DeserializeObject<Thongtinnganhang>(bankJson);
                    }
                    else
                    {
                        var banks = JsonConvert.DeserializeObject<List<Thongtinnganhang>>(bankJson);
                        bankInfo = banks.FirstOrDefault(); // Lấy tài khoản đầu tiên nếu có nhiều
                    }
                }

                if (bankInfo == null || bankInfo.bankid == 0)
                {
                    ModelState.AddModelError("", "Không tìm thấy tài khoản ngân hàng.");
                    return View("EditAccountBank", model);
                }
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                // 🔹 Gửi request `PUT` đến API
                var response = await _httpClient.PutAsync($"https://localhost:7126/api/Thanhtoan/update/{bankInfo.bankid}", httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Phản hồi từ API: " + responseContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật tài khoản ngân hàng thành công!";
                    return RedirectToAction("Profile");
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi API: " + responseContent);
                    return View("EditAccountBank", model); // ✅ Trả về view chỉnh sửa nếu có lỗi
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                return View("EditAccountBank", model);
            }
        }

    }
}
