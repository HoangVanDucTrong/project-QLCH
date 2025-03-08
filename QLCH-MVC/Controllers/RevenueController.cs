using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using QLCH_MVC.Models;
namespace QLCH_MVC.Controllers
{
    public class RevenueController : Controller
    {
        private readonly HttpClient _httpClient;
        public RevenueController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7126");
        }
        public async Task<IActionResult> Index(string filterType = "daily", string fromDate = null, string toDate = null)
        {
            var userRoles = HttpContext.Session.GetString("UserRoles");
            var roles = userRoles?.Split(',') ?? new string[0];
            ViewBag.UserRoles = roles;
            ViewBag.FilterType = filterType;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            var token = HttpContext.Session.GetString("JWTToken");

            // Xây dựng API URL với tham số lọc
            string apiUrl = $"api/revenue/{filterType}?";
            if (!string.IsNullOrEmpty(fromDate)) apiUrl += $"fromDate={fromDate}&";
            if (!string.IsNullOrEmpty(toDate)) apiUrl += $"toDate={toDate}";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewBag.Message = "Không có dữ liệu!";
                return View(new List<RevenueData>());
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var revenueData = JsonConvert.DeserializeObject<List<RevenueData>>(jsonResponse);
                return View(revenueData);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return View("Error");
            }
        }
    }
}


