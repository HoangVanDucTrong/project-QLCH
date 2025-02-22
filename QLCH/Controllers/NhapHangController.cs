using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLCH.Data;
using QLCH.Models;
using QLCH.Models.IRepository;
using Microsoft.EntityFrameworkCore;


namespace QLCH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NhapHangController : ControllerBase
    {
        private readonly QLCHDbConText _context;
        private readonly ILogger<NhapHangController> _logger;
        private readonly IGetClaimsFromToken _laytoken;
        public NhapHangController(QLCHDbConText context, ILogger<NhapHangController> logger, IGetClaimsFromToken laytoken)
        {
            _context = context;
            _logger = logger;
            _laytoken = laytoken;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhapHang>>> Getnhaphang()
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

            var nhaphang = await _context.nhapHangs
                .Where(sp => sp.StoreId == storeId)
                .ToListAsync();

            if (nhaphang == null || !nhaphang.Any())
            {
                _logger.LogInformation("No products found for StoreId: {storeId}");
                return NotFound(); // Không tìm thấy sản phẩm nào
            }

            _logger.LogInformation("Successfully retrieved products.");
            return Ok(nhaphang);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<NhapHang>> Getnhaphang(int id)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            // Tìm sản phẩm theo id và StoreId
            var sp = await _context.nhapHangs
                .Where(s => s.NHid == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (sp == null)
            {
                return NotFound();
            }

            return Ok(sp);
        }
        [HttpPost]

        public async Task<ActionResult<NhapHang>> PostNhapHang([FromForm] NhapHang cl)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }



            cl.StoreId = int.Parse(storeId);
            cl.ThanhTien = cl.DonGia  * cl.soluong;
            _context.nhapHangs.Add(cl);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return CreatedAtAction(nameof(Getnhaphang), new { id = cl.NHid }, cl);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Putnhaphang(int id, [FromForm] NhapHang  cl)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);
            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            // Tìm sản phẩm trong cơ sở dữ liệu
            var existingCalam = await _context.nhapHangs
                .Where(s => s.NHid == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (existingCalam == null)
            {
                return NotFound();
            }
            // Cập nhật các thuộc tính khác từ yêu cầu
            existingCalam.TenHangHoa = cl.TenHangHoa;
            existingCalam.DonVi = cl.DonVi;
            existingCalam.DonGia = cl.DonGia;
            _context.Entry(existingCalam).State = EntityState.Modified;
            await _context.SaveChangesAsync(); // Sử dụng await để chờ hoàn thành lưu thay đổi

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletenhaphang(int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            var cl = await _context.nhapHangs
                .Where(s => s.NHid == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (cl == null)
            {
                return NotFound();
            }

            _context.nhapHangs.Remove(cl);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
