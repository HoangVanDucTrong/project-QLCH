
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCH.Data;
using QLCH.Models;
using QLCH.Models.DTO;
using QLCH.Models.IRepository;
using QLCH.Models.Repository;

namespace QLCH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountStaff : Controller
    {
        private readonly QLCHDbConText _context;
        private readonly UserManager<AuthorcationStore> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly IGetClaimsFromToken _getClaimsFromToken;
        private readonly ILogger<AccountStaff> _logger;

        public AccountStaff(UserManager<AuthorcationStore> userManager, QLCHDbConText context, IGetClaimsFromToken getClaimsFromToken, ITokenRepository tokenRepository, ILogger<AccountStaff> logger)
        {
            _userManager = userManager;
            _context = context;
            _getClaimsFromToken = getClaimsFromToken;
            _tokenRepository = tokenRepository;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("checkAccountnv")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> checkAccountnv([FromQuery] int NVid)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var storeIdString = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            if (!int.TryParse(storeIdString, out var storeId))
            {
                return BadRequest("StoreId is not valid.");
            }

            // Kiểm tra tài khoản trong cơ sở dữ liệu
            var existingAccount = await _context.taiKhoanNhanViens
                .FirstOrDefaultAsync(a => a.NVid == NVid);

            // Nếu tài khoản tồn tại, trả về trạng thái có tài khoản
            if (existingAccount != null)
            {
                return Ok(new { status = "Hoạt động " });
            }

            // Nếu tài khoản không tồn tại, trả về trạng thái chưa có tài khoản
            return Ok(new { status = "Chưa hoạt động" });
        }
        
        [HttpPost]
        [Route("CreateEmployeeAccount")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateEmployeeAccount(TaiKhoanNhanVien model)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var claimsPrincipal = _getClaimsFromToken.laytoken(token);

            var storeIdString = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            if (!int.TryParse(storeIdString, out var storeId))
            {
                return BadRequest("StoreId is not valid.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingAccount = await _context.taiKhoanNhanViens
      .FirstOrDefaultAsync(a => a.NVid == model.NVid);

            if (existingAccount != null)
            {
                return BadRequest("Nhân viên này đã có tài khoản.");
            }

            var user = new AuthorcationStore
            {
                UserName = model.TenDangNhap,
                DiaChi = model.DiaChi ?? "Default Address",
                Sdt = model.Sdt ?? "Default numberphone",
                QuocGia = model.QuocGia ?? "Default QUOCGIA",
            };

            var result = await _userManager.CreateAsync(user, model.MatKhau);
            if (result.Succeeded)
            {
                // Thêm nhân viên vào role "Employee"
                await _userManager.AddToRoleAsync(user, "Employee");

                // Thêm vào cơ sở dữ liệu
                var nv = new TaiKhoanNhanVien
                {
                    TenDangNhap = user.UserName,
                    MatKhau = user.PasswordHash,
                    StoreId = storeId,
                    NVid = model.NVid
                };

                _context.taiKhoanNhanViens.Add(nv);
                await _context.SaveChangesAsync();

                return Ok();
            }

            // Xử lý lỗi nếu tạo người dùng không thành công
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("EmployeeLogin")]
        public async Task<IActionResult> EmployeeLogin([FromBody] LoginRequestDTO loginRequestDTO)
        {
       

            // Tìm kiếm người dùng dựa trên tên đăng nhập
            var user = await _userManager.FindByNameAsync(loginRequestDTO.Username);

            // Kiểm tra nếu người dùng tồn tại
            if (user == null)
            {
                
                return BadRequest("Username not found.");
            }

        

            // Kiểm tra mật khẩu
            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (!checkPasswordResult)
            {
              
                return BadRequest("Password incorrect.");
            }

            // Lấy danh sách vai trò của người dùng
            var roles = await _userManager.GetRolesAsync(user);
            Console.WriteLine("quuyeenf tr ve la: " + string.Join(", ", roles));


            // Kiểm tra nếu người dùng có vai trò "Employee"
            if (!roles.Contains("Employee"))
            {
              
                return Unauthorized("You do not have permission to access this resource.");
            }

            // Tìm kiếm NVid trong bảng TaiKhoanNhanVien sử dụng Username
            var employeeAccount = await _context.taiKhoanNhanViens
    .FirstOrDefaultAsync(nv => nv.TenDangNhap == loginRequestDTO.Username);
          

            if (employeeAccount == null)
            {
                return BadRequest("Employee account not found.");
            }
            if (employeeAccount.NVid == null)
            {
                return BadRequest("NVid is null.");
            }

            _logger.LogInformation("Employee account found for username: {Username} with NVid: {NVid}", loginRequestDTO.Username, employeeAccount.NVid);

            // Tìm kiếm NhanVien dựa trên NVid
            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(nv => nv.NVid == employeeAccount.NVid);
            if (nhanVien == null)
            {
                return BadRequest("Employee not found.");
            }

            if (!nhanVien.StoreId.HasValue)
            {
                return BadRequest("StoreId is null.");
            }

            if (nhanVien == null || nhanVien.StoreId == null)
            {
                return BadRequest("Employee not found or StoreId is null.");
            }

            var idstore = nhanVien.StoreId.Value; // Sử dụng .Value sau khi đảm bảo không null


            // Tạo JWT token với StoreId
            var jwtToken = _tokenRepository.CreateJWTToken2(user, roles.ToList(), idstore);
            _logger.LogInformation("JWT token created for user: {Username}", loginRequestDTO.Username);

            var response = new LoginResponseDTO
            {
                JwtToken = jwtToken,
                Roles = roles.ToList()
            };

            return Ok(response); // Trả về chuỗi token
        }
    }
}
