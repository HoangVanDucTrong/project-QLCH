using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCH.Data;
using QLCH.Models;
using SendGrid.Helpers.Mail;
using System.Numerics;

namespace QLCH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Danhmucsanpham : ControllerBase
    {
        private readonly QLCHDbConText _dbcontext;
        public Danhmucsanpham(QLCHDbConText dbcontext)
        {
            _dbcontext = dbcontext;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DanhMuc>>> Getdanhmuc()
        {
           
            return await _dbcontext.DanhMucs.ToListAsync();
        }
        [HttpPost]
        public async Task<ActionResult<DanhMuc>> PostDanhMuc([FromBody] DanhMuc danhMuc)
        {
            // Không cần gửi thông tin sanPhams khi tạo danh mục mới
           

            _dbcontext.DanhMucs.Add(danhMuc);
            await _dbcontext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDanhMucById), new { id = danhMuc.DanhMucId }, danhMuc);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DanhMuc>> GetDanhMucById(int id)
        {
            var danhMuc = await _dbcontext.DanhMucs
                                         .FirstOrDefaultAsync(dm => dm.DanhMucId == id);

            if (danhMuc == null)
            {
                return NotFound();
            }

            return danhMuc;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDanhMuc(int id, DanhMuc danhMuc)
        {
            if (id != danhMuc.DanhMucId)
            {
                return BadRequest("DanhMuc ID mismatch");
            }

            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDanhMuc = await _dbcontext.DanhMucs.FindAsync(id);
            if (existingDanhMuc == null)
            {
                return NotFound($"DanhMuc with ID '{id}' not found.");
            }

            existingDanhMuc.TenDanhMuc = danhMuc.TenDanhMuc;
            existingDanhMuc.MoTa = danhMuc.MoTa;

            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency exception (e.g., logging)
                return StatusCode(500, "An error occurred while updating the DanhMuc");
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDanhMuc(int id)
        {
            var danhMuc = await _dbcontext.DanhMucs.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound($"DanhMuc with ID '{id}' not found.");
            }

            _dbcontext.DanhMucs.Remove(danhMuc);

            try
            {
                await _dbcontext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Handle exception (e.g., logging)
                return StatusCode(500, "An error occurred while deleting the DanhMuc");
            }

            return NoContent();
        }

    }
}
