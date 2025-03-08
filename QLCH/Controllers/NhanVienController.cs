
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCH.Data;
using QLCH.Models;
using QLCH.Models.IRepository;

namespace QLCH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class NhanVienController : ControllerBase
    {
        private readonly IGetClaimsFromToken _getClaimsFromToken;
        private readonly QLCHDbConText _context;

        public NhanVienController(QLCHDbConText context, ILogger<Sanphamcontroller> logger, IGetClaimsFromToken getClaimsFromToken)
        {
            _context = context;

            _getClaimsFromToken = getClaimsFromToken;
        }
        [HttpGet("Getnhanvien")]
        public async Task<ActionResult<IEnumerable<NhanVien>>> Getnhanvien()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var storeIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
          

            if (string.IsNullOrEmpty(storeIdClaim))
            {
               
                return Unauthorized(); // Không có StoreId trong claims
            }

            if (!int.TryParse(storeIdClaim, out int storeId))
            {
             
                return BadRequest("Invalid StoreId"); // StoreId không hợp lệ
            }
        var NV = await _context.NhanViens
                .Where(sp => sp.StoreId == storeId)
                .ToListAsync();

            if (NV == null || !NV.Any())
            {
              
                return NotFound(); // Không tìm thấy sản phẩm nào
            }

           
            return Ok(NV);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVien>> Getnhanvienid(int id)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var nhanvien = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(nhanvien))
            {
                return Unauthorized();
            }

            // Tìm sản phẩm theo id và StoreId
            var sp = await _context.NhanViens
                .Where(s => s.NVid == id && s.StoreId == int.Parse(nhanvien))
                .FirstOrDefaultAsync();

            if (sp == null)
            {
                return NotFound();
            }

            return Ok(sp);
        }

        [HttpPost("Postnhanvien")]
        public async Task<ActionResult<NhanVien>> Postnhanvien([FromForm] NhanVien NV, IFormFile ImageURL)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            // Kiểm tra Ngày Vào Làm
            if (NV.NgayVaoLam == default(DateTime))
            {
                return BadRequest("Ngày vào làm không được để trống.");
            }
            /*
            if (NV.NgayVaoLam > DateTime.Now)
            {
                return BadRequest("Ngày vào làm không được sau ngày hiện tại.");
            }*/

            if (NV.NgayVaoLam < new DateTime(1900, 1, 1))
            {
                return BadRequest("Ngày vào làm không được quá xa trong quá khứ.");
            }
            var isPhoneExists = await _context.NhanViens.AnyAsync(nv => nv.SDT == NV.SDT);
            if (isPhoneExists)
            {
                return BadRequest("Số điện thoại đã tồn tại. Vui lòng sử dụng số điện thoại khác.");
            }

            // Mã hóa ảnh thành chuỗi Base64
            using (var memoryStream = new MemoryStream())
            {
                await ImageURL.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                NV.AnhNhanVien = Convert.ToBase64String(imageBytes);
            }

            NV.StoreId = int.Parse(storeId);

            _context.NhanViens.Add(NV);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return CreatedAtAction(nameof(Getnhanvienid), new { id = NV.NVid }, NV);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Putnhanvien(int id, [FromForm] NhanVien nv, IFormFile ImageURL)
        {


       
            // Lấy token từ header và phân tích claims
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var nhanvien = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            // Tìm sản phẩm trong cơ sở dữ liệu
            var existingNhanvien = await _context.NhanViens
                .Where(s => s.NVid == id && s.StoreId == int.Parse(nhanvien))
                .FirstOrDefaultAsync();

            if (existingNhanvien == null)
            {
                return NotFound();
            }

            // Mã hóa ảnh thành chuỗi Base64
            using (var memoryStream = new MemoryStream())
            {
                await ImageURL.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                existingNhanvien.AnhNhanVien = Convert.ToBase64String(imageBytes);
            }

            // Cập nhật các thuộc tính khác từ yêu cầu
            existingNhanvien.TenNV = nv.TenNV;
            existingNhanvien.SDT = nv.SDT;
            existingNhanvien.NgayVaoLam = nv.NgayVaoLam;
            existingNhanvien.MucLuong = nv.MucLuong;
            _context.Entry(existingNhanvien).State = EntityState.Modified;
            await _context.SaveChangesAsync(); // Sử dụng await để chờ hoàn thành lưu thay đổi

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletenhanvien(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var claimsPrincipal = _getClaimsFromToken.laytoken(token);

                var nhanvien = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

                if (string.IsNullOrEmpty(nhanvien))
                {
                    return Unauthorized();
                }

                // Xóa các bản ghi liên quan trong bảng TaiKhoanNhanVien
                var relatedAccounts = _context.taiKhoanNhanViens.Where(t => t.NVid == id);
                if (relatedAccounts.Any())
                {
                    _context.taiKhoanNhanViens.RemoveRange(relatedAccounts);
                    await _context.SaveChangesAsync(); // Lưu thay đổi trước khi xóa nhân viên
                }

                // Xóa các bản ghi liên quan trong bảng CaLamNhanVien
                var relatedCaLam = _context.caLamNhanViens.Where(c => c.NVid == id);
                if (relatedCaLam.Any())
                {
                    _context.caLamNhanViens.RemoveRange(relatedCaLam);
                    await _context.SaveChangesAsync(); // Lưu thay đổi trước khi xóa nhân viên
                }

                // Xóa nhân viên
                var sp = await _context.NhanViens
                    .Where(s => s.NVid == id && s.StoreId == int.Parse(nhanvien))
                    .FirstOrDefaultAsync();

                if (sp == null)
                {
                    return NotFound();
                }

                _context.NhanViens.Remove(sp);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await transaction.RollbackAsync();
                return StatusCode(500, $"Lỗi khi xóa nhân viên: {ex.Message}");
            }
        }



    }
}
