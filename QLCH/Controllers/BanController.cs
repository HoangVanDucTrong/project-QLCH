using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCH.Data;
using QLCH.Models;
using QLCH.Models.IRepository;

namespace QLCH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BanController : ControllerBase
    {
        private readonly QLCHDbConText _context;
        private readonly ILogger<BanController> _logger;
        private readonly IGetClaimsFromToken _laytoken;
        public BanController(QLCHDbConText context, ILogger<BanController> logger, IGetClaimsFromToken laytoken)
        {
            _context = context;
            _logger = logger;
            _laytoken = laytoken;
        }
        [HttpGet]
        [Route("Getban")]
        public async Task<ActionResult<IEnumerable<Bans>>> GetBan()
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

            var ban = await _context.bans
                .Where(sp => sp.StoreId == storeId)
                .ToListAsync();

            if (ban == null || !ban.Any())
            {
                _logger.LogInformation("Cửa hàng của bạn chưa có bàn!Hãy thêm bàn ngay!");
                return NotFound(); // Không tìm thấy sản phẩm nào
            }

            _logger.LogInformation("Successfully retrieved products.");
            return Ok(ban);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Bans>> Getbanid(int id)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            // Tìm sản phẩm theo id và StoreId
            var sp = await _context.bans
                .Where(s => s.BanId == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (sp == null)
            {
                return NotFound();
            }

            return Ok(sp);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Bans>> PostBan([FromForm] Bans cl)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }



            cl.StoreId = int.Parse(storeId);
            bool isTableExists = await _context.bans
      .AnyAsync(b => b.SoBan == cl.SoBan && b.StoreId == cl.StoreId);
            if (isTableExists)
            {
                return BadRequest(new { message = "Số bàn đã tồn tại trong cửa hàng này!" });
            }

            _context.bans.Add(cl);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

            return CreatedAtAction(nameof(GetBan), new { id = cl.BanId }, cl);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Putban(int id, [FromForm] Bans cl)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);
            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            // Tìm sản phẩm trong cơ sở dữ liệu
            var existingban = await _context.bans
                .Where(s => s.BanId == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (existingban == null)
            {
                return NotFound();
            }
            // Cập nhật các thuộc tính khác từ yêu cầu
            existingban.SoBan = cl.SoBan;
       
            _context.Entry(existingban).State = EntityState.Modified;
            await _context.SaveChangesAsync(); // Sử dụng await để chờ hoàn thành lưu thay đổi

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Deleteban(int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            var cl = await _context.bans
                .Where(s => s.BanId == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (cl == null)
            {
                return NotFound();
            }

            _context.bans.Remove(cl);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
