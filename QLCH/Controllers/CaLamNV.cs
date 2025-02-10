using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCH.Data;
using QLCH.Models;
using QLCH.Models.IRepository;
using QLCH.Models.Repository;

namespace QLCH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CaLamNV : ControllerBase
    {
        private readonly QLCHDbConText _context;
        private readonly ILogger<Sanphamcontroller> _logger;
        private readonly IGetClaimsFromToken _laytoken;
        public CaLamNV(QLCHDbConText context, ILogger<Sanphamcontroller> logger, IGetClaimsFromToken laytoken)
        {
            _context = context;
            _logger = logger;
            _laytoken = laytoken;
        }

        [HttpGet("GetCaLam")]
        public async Task<ActionResult<IEnumerable<object>>> GetCaLam()
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

            // Lấy ngày hôm nay và tính toán ngày bắt đầu (Thứ 2) và ngày kết thúc (Chủ nhật) của tuần hiện tại
            var today = DateTime.Today;

            // Nếu hôm nay là Chủ Nhật, ta cần tính lùi lại 6 ngày để đến Thứ Hai của tuần hiện tại
            var daysToStartOfWeek = (today.DayOfWeek == DayOfWeek.Sunday) ? -6 : -(int)today.DayOfWeek + (int)DayOfWeek.Monday;

            var startOfWeek = today.AddDays(daysToStartOfWeek); // Ngày đầu tuần (Thứ 2)
            var endOfWeek = startOfWeek.AddDays(6); // Ngày cuối tuần (Chủ nhật)

        


            // Lọc dữ liệu ca làm trong tuần hiện tại
            var calam = await _context.caLamNhanViens
                .Where(cl => cl.StoreId == storeId && cl.NgayLam >= startOfWeek && cl.NgayLam <= endOfWeek)
                .Join(
                    _context.NhanViens, // Thực hiện JOIN với bảng NhanVien
                    cl => cl.NVid,      // Khóa liên kết từ bảng CaLamNhanVien
                    nv => nv.NVid,      // Khóa liên kết từ bảng NhanVien
                    (cl, nv) => new     // Chọn kết quả mong muốn
                    {
                        cl.CaLamId,
                        cl.NgayLam,
                        cl.GioBatDau,
                        cl.GioKetThuc,
                        cl.GhiChu,
                        cl.Thu,
                        cl.calam,
                        TenNhanVien = nv.TenNV, // Lấy tên nhân viên từ bảng NhanVien
                        nv.SDT,                // Có thể thêm các cột khác từ NhanVien nếu cần
                        nv.StoreId,
                        StartOfWeek = startOfWeek, // Thêm startOfWeek
                        EndOfWeek = endOfWeek
                    })
                .ToListAsync();

            if (calam == null || !calam.Any())
            {
                _logger.LogInformation($"No work shifts found for StoreId: {storeId}");
                return NotFound(); // Không tìm thấy ca làm nào
            }

            _logger.LogInformation("Successfully retrieved work shifts.");
            return Ok(calam);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SanPham>> GetCalamid(int id)
        {

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            // Tìm sản phẩm theo id và StoreId
            var sp = await _context.caLamNhanViens
                .Where(s => s.CaLamId == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (sp == null)
            {
                return NotFound();
            }

            return Ok(sp);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CaLamNhanVien>> PostCaLam([FromBody] List<CaLamNhanVien> caLamList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);

            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            foreach (var caLam in caLamList)
            {
                if (caLam.NVid <= 0 || string.IsNullOrEmpty(caLam.calam) || caLam.GioBatDau == null || caLam.GioKetThuc == null)
                {
                    return BadRequest("Thông tin ca làm không hợp lệ.");
                }

                // Kiểm tra trùng lặp lịch làm
                var isDuplicate = _context.caLamNhanViens.Any(cl =>
                    cl.StoreId == int.Parse(storeId) &&
                    cl.NVid == caLam.NVid &&
                    cl.NgayLam == caLam.NgayLam &&
                    cl.calam == caLam.calam);

                if (isDuplicate)
                {
                    return BadRequest(new
                    {
                        Message = "Lịch làm bị trùng lặp.",
                        DuplicateInfo = new
                        {
                            caLam.NVid,
                            caLam.NgayLam,
                            caLam.calam
                        }
                    });
                }

                // Thêm dữ liệu vào bảng
                _context.caLamNhanViens.Add(new CaLamNhanVien
                {
                    NVid = caLam.NVid,
                    StoreId = int.Parse(storeId),
                    NgayLam = caLam.NgayLam,
                    GioBatDau = caLam.GioBatDau,
                    GioKetThuc = caLam.GioKetThuc,
                    GhiChu = caLam.GhiChu,
                    calam = caLam.calam,
                    Thu = GetThuFromDate(caLam.NgayLam)
                });
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lưu dữ liệu: {ex.Message}");
            }
        }

        private string GetThuFromDate(DateTime date)
        {
            var thuArray = new[] { "Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7" };
            return thuArray[(int)date.DayOfWeek];
        }
        [HttpGet("Filter")]
        public async Task<IActionResult> Filter(DateTime startDate, DateTime endDate)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);

            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (!int.TryParse(storeId, out int storeIdInt))
            {
                return BadRequest(new { Message = "Invalid StoreId." });
            }
            var startdate = startDate;
            var enddate = endDate;  
            // Lấy danh sách các ca làm nằm trong khoảng thời gian
            var filteredData = await _context.caLamNhanViens
                .Where(cl => cl.StoreId == storeIdInt && cl.NgayLam >= startDate && cl.NgayLam <= endDate)
                .Join(
                    _context.NhanViens, // Thực hiện JOIN với bảng NhanVien
                    cl => cl.NVid,      // Khóa liên kết từ bảng CaLamNhanVien
                    nv => nv.NVid,      // Khóa liên kết từ bảng NhanVien
                    (cl, nv) => new     // Chọn kết quả mong muốn
                    {
                        cl.CaLamId,
                        cl.NgayLam,
                        cl.GioBatDau,
                        cl.GioKetThuc,
                        cl.GhiChu,
                        cl.Thu,
                        cl.calam,
                        TenNhanVien = nv.TenNV, // Lấy tên nhân viên từ bảng NhanVien
                        nv.SDT,                // Có thể thêm các cột khác từ NhanVien nếu cần
                        nv.StoreId,
                        StartOfWeek= startdate,
                        EndOfWeek= enddate
                    })
                .ToListAsync();


            if (!filteredData.Any())
            {
                return NotFound(new { Message = "No data found for the given date range." });
            }

            // Trả về danh sách đã lọc dưới dạng JSON
            return Ok(filteredData);
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCaLam([FromBody] List<CaLamNhanVien> caLamList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);
            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }
            foreach (var caLam in caLamList)
            {
                Console.WriteLine($"CaLamId: {caLam.CaLamId}, StoreId: {storeId}");
            }

            foreach (var caLam in caLamList)
            {
                var existingCalam = await _context.caLamNhanViens
                    .FirstOrDefaultAsync(cl => cl.CaLamId == caLam.CaLamId && cl.StoreId == int.Parse(storeId));

                if (existingCalam == null)
                {
                    return NotFound($"Không tìm thấy ca làm với ID: {caLam.CaLamId}");
                }

                existingCalam.NgayLam = caLam.NgayLam;
                existingCalam.GioBatDau = caLam.GioBatDau;
                existingCalam.GioKetThuc = caLam.GioKetThuc;
                existingCalam.Thu = GetThuFromDate(caLam.NgayLam);
                existingCalam.calam = caLam.calam;
                existingCalam.GhiChu = caLam.GhiChu;

                _context.Entry(existingCalam).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi cập nhật: {ex.Message}");
            }
           
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCalam(int id)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _laytoken.laytoken(token);



            var storeId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;

            if (string.IsNullOrEmpty(storeId))
            {
                return Unauthorized();
            }

            var cl = await _context.caLamNhanViens
                .Where(s => s.CaLamId == id && s.StoreId == int.Parse(storeId))
                .FirstOrDefaultAsync();

            if (cl == null)
            {
                return NotFound();
            }

            _context.caLamNhanViens.Remove(cl);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
