
﻿using Microsoft.AspNetCore.Mvc;
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
    public class Sanphamcontroller : ControllerBase
    {
        private readonly QLCHDbConText _context;
        private readonly ILogger<Sanphamcontroller> _logger;
        private readonly IGetClaimsFromToken _laytoken;
        public Sanphamcontroller(QLCHDbConText context, ILogger<Sanphamcontroller> logger,IGetClaimsFromToken laytoken)
        {
            _context = context;
            _logger = logger;
            _laytoken= laytoken;
        }
       
        [HttpGet]
        [Route("Getsanpham")]
        public async Task<ActionResult<IEnumerable<SanPham>>> Getsanpham()
        {


            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);

            var storeIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            _logger.LogInformation($"StoreIdClaim: {storeIdClaim}");

            if (string.IsNullOrEmpty(storeIdClaim))
            {
                _logger.LogWarning("StoreIdClaim is null or empty.");
                return Unauthorized(); // Không có StoreId trong claims
            }

            if (!int.TryParse(storeIdClaim, out int storeId))
            {
                _logger.LogWarning($"Failed to parse StoreId: {storeIdClaim}");
                return BadRequest("Invalid StoreId"); // StoreId không hợp lệ
            }
            
            var sanpham = await _context.SanPhams
                .Where(sp => sp.StoreId == storeId)
                .ToListAsync();

            if (sanpham == null || !sanpham.Any())
            {
                _logger.LogInformation("No products found for StoreId: {storeId}");
                return NotFound(); // Không tìm thấy sản phẩm nào
            }

            _logger.LogInformation("Successfully retrieved products.");
            return Ok(sanpham);
        }
        
       
        [HttpGet("{id}")]
        public async Task<ActionResult<SanPham>> getsanphamid(int id)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            // Tìm sản phẩm theo id và StoreId
            var sp = await _context.SanPhams
                .Where(s => s.SanPhamId == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (sp == null)
            {
                return NotFound();
            }

            return Ok(sp);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<ActionResult<SanPham>> Postsanpham([FromForm] SanPham sp, IFormFile ImageURL)
        {
            if (ImageURL == null)
            {
                return BadRequest("Image file is required.");
            }

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            // Mã hóa ảnh thành chuỗi Base64
            using (var memoryStream = new MemoryStream())
            {
                await ImageURL.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                sp.ImageBase64 = Convert.ToBase64String(imageBytes);
            }
            
            sp.StoreId = int.Parse(storeId);

            _context.SanPhams.Add(sp);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return CreatedAtAction(nameof(getsanphamid), new { id = sp.SanPhamId }, sp);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> PutSanpham(int id, [FromForm] SanPham sp, IFormFile ImageURL)
        {
     

            // Kiểm tra xem hình ảnh có được cung cấp không
            if (ImageURL == null)
            {
                return BadRequest("Image file is required.");
            }

            // Lấy token từ header và phân tích claims
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);
            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            // Tìm sản phẩm trong cơ sở dữ liệu
            var existingSanpham = await _context.SanPhams
                .Where(s => s.SanPhamId == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (existingSanpham == null)
            {
                return NotFound();
            }

            // Mã hóa ảnh thành chuỗi Base64
            using (var memoryStream = new MemoryStream())
            {
                await ImageURL.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                existingSanpham.ImageBase64 = Convert.ToBase64String(imageBytes);
            }

            // Cập nhật các thuộc tính khác từ yêu cầu
            existingSanpham.Ten = sp.Ten;
            existingSanpham.Gia = sp.Gia;
            existingSanpham.MoTa = sp.MoTa;
            _context.Entry(existingSanpham).State = EntityState.Modified;
            await _context.SaveChangesAsync(); // Sử dụng await để chờ hoàn thành lưu thay đổi

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> DeleteSanpham(int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);
            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            // Xóa các bản ghi liên quan trong bảng SanPhamDonHang trước
            var relatedOrders = _context.sanPhamDonHangs.Where(s => s.SanPhamId == id);
            _context.sanPhamDonHangs.RemoveRange(relatedOrders);

            var sp = await _context.SanPhams
                .Where(s => s.SanPhamId == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (sp == null)
            {
                return NotFound();
            }

            _context.SanPhams.Remove(sp);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("api/sanpham/search")]
        public async Task<IActionResult> SearchSanPham(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Nếu không có từ khóa, trả về danh sách rỗng
                return Ok(new List<object>()); // Trả về danh sách rỗng
            }

            // Tìm kiếm theo từ khóa (không phân biệt hoa thường) và giới hạn số lượng gợi ý
            var sanPhams = await _context.SanPhams
                .Where(sp => sp.Ten.ToLower().Contains(query.ToLower())) // Tìm kiếm không phân biệt chữ hoa/thường
                .Select(sp => new
                {
                    sp.SanPhamId,      // Lấy ID sản phẩm
                    sp.Ten,            // Lấy tên sản phẩm
                    sp.Gia,            // Lấy giá sản phẩm
                    sp.ImageBase64,    // Lấy chuỗi ảnh dạng Base64
                })
                .Take(10) // Giới hạn số lượng gợi ý trả về (ở đây là 10 sản phẩm)
                .ToListAsync();

            return Ok(sanPhams); // Trả về danh sách dưới dạng JSON
        }


    }
}
