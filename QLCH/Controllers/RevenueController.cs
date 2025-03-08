using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCH.Data;
using QLCH.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using QLCH.Models.IRepository;

namespace QLCH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RevenueController : ControllerBase
    {
        private readonly QLCHDbConText _context;
        private readonly ILogger<Sanphamcontroller> _logger;
        private readonly IGetClaimsFromToken _laytoken;
        public RevenueController(QLCHDbConText context, IGetClaimsFromToken laytoken)
        {
            _context = context;  
            _laytoken = laytoken;
        }
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyRevenue(DateTime? fromDate, DateTime? toDate)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);

            var storeIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeIdClaim) || !int.TryParse(storeIdClaim, out int storeId))
            {
                return Unauthorized("StoreId is missing or invalid");
            }

            var query = _context.transactions
                .Where(t => t.CreatedAt.HasValue && t.StoreId == storeId);

            // Lọc theo ngày
            if (fromDate.HasValue)
                query = query.Where(t => t.CreatedAt.Value.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(t => t.CreatedAt.Value.Date <= toDate.Value.Date);

            var revenue = await query
                .GroupBy(t => t.CreatedAt.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .OrderBy(r => r.Date)
                .ToListAsync();

            return Ok(revenue);
        }

        // API lấy tổng doanh thu theo tháng
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);

            var storeIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var revenue = await _context.transactions
     .Where(t => t.CreatedAt.HasValue) // Kiểm tra null
     .GroupBy(t => new
     {
         Year = t.CreatedAt.Value.Year,
         Month = t.CreatedAt.Value.Month
     })
     .Select(g => new
     {
         Year = g.Key.Year,
         Month = g.Key.Month,
         TotalAmount = g.Sum(t => t.Amount)
     })
     .OrderBy(r => r.Year).ThenBy(r => r.Month)
     .ToListAsync();


            return Ok(revenue);
        }
    }
}
