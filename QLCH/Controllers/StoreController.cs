using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLCH.Data;
using QLCH.Models;
using QLCH.Models.DTO;
using QLCH.Models.IRepository;
using QLCH.Models.Repository;
using QLCH.Models.resetpassword;
using QLCH.SMTPEMAIL;
using System;
using System.Net;
namespace QLCH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly UserManager<AuthorcationStore> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly QLCHDbConText _context;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StoreController(UserManager<AuthorcationStore> userManager, ITokenRepository tokenRepository, QLCHDbConText context, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenRepository = tokenRepository;
            _context = context;
            _emailSender = emailSender;
        }
       
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerRequestDTO.Username);
                if (existingUser != null)
                {
                    return BadRequest("Tài khoản đã tồn tại với email này.");
                }
                var authorcationStore = new AuthorcationStore
                {
                    UserName = registerRequestDTO.Username,
                    Email = registerRequestDTO.Username,
                    Sdt = registerRequestDTO.Sdt,
                    DiaChi = registerRequestDTO.DiaChi,
                    QuocGia = registerRequestDTO.QuocGia,
                };

                var identityResult = await _userManager.CreateAsync(authorcationStore, registerRequestDTO.Password);
                if (identityResult.Succeeded)
                {
                  
                      
                        
                            identityResult = await _userManager.AddToRolesAsync(authorcationStore, new[] { "Admin" });
                         
                        
                        // Log khi tạo mã xác nhận
                        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(authorcationStore);
                        var encodedToken = WebUtility.UrlEncode(confirmationToken);
                  

                        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Store", new { userId = authorcationStore.Id, token = WebUtility.UrlEncode(confirmationToken) }, Request.Scheme);
                    
                        string emailBody = $@"
                            <h2>Confirm Your Email</h2>
                            <p>Please confirm your email by clicking the button below:</p>
                            <a href='{confirmationLink}' style='display: inline-block;
                             padding: 10px 20px; font-size: 16px; color: #fff;
                                background-color: #007bff; text-decoration: none; border-radius: 5px;'>Confirm Email</a>
                            <p>If you did not request this, please ignore this email.</p>
                        ";

                        await _emailSender.SendEmailAsync(registerRequestDTO.Username, "Confirm your email", emailBody);

                        return Ok("Register Successful! Please check your email to confirm your account.");
                    }
                
                return BadRequest("Something wrong!");
            }
            catch (Exception ex)
            {
                // Log exception details for debugging
                // Use your preferred logging framework here, e.g., Serilog, NLog, etc.
                Console.WriteLine($"Error during registration: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Invalid email confirmation request.");
            }

            var decodedToken = WebUtility.UrlDecode(token);

            Console.WriteLine($"Decoded Token: {decodedToken}");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

         

            if (result.Succeeded)
            {
                // Kiểm tra nếu email đã tồn tại trong bảng Stores
                var existingStore = await _context.Stores.FirstOrDefaultAsync(s => s.Email == user.Email);
                if (existingStore != null)
                {
                    // Nếu email đã tồn tại, trả về lỗi
                    return BadRequest("Your account has been created!");
                }

                var store = new store
                {
                    Email = user.Email,
                    Sdt = user.Sdt,
                    DiaChi = user.DiaChi,
                    QuocGia = user.QuocGia,
                    Pass = user.PasswordHash
                };

                _context.Stores.Add(store);
                await _context.SaveChangesAsync();

                return Ok("Email confirmed successfully and account created!");
            }
            return BadRequest("Error confirming your email.");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDTO.Username);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    return BadRequest("You need to confirm your email to log in.");
                }

                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
                if (checkPasswordResult)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    Console.WriteLine("quuyeenf tr ve la: " + string.Join(", ", roles));

                    if (roles != null)
                    {
                        var store = await _context.Stores
                 .FirstOrDefaultAsync(s => s.Email == user.Email); // Thay đổi điều kiện truy vấn cho phù hợp

                        if (store == null)
                        {
                            return BadRequest("Store not found for the user.");
                        }

                        var storeId = store.StoreId;
                        var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList(), storeId);
                        Console.WriteLine("chuoi:"+jwtToken.ToString());
                        var response = new LoginResponseDTO
                        {
                            JwtToken = jwtToken,
                            Roles = roles.ToList() 
                        };
                        return Ok(response); // Trả về chuỗi token
                    }
                }
            }
            return BadRequest("Username or password incorrect");
        }
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
                return BadRequest("User not found.");

            // Tạo mã số ngẫu nhiên 6 chữ số
            var randomCode = new Random().Next(100000, 999999).ToString();

            // Lưu mã số vào cơ sở dữ liệu
            var passwordResetCode = new PasswordResetCode
            {
                Email = forgotPasswordModel.Email,
                Code = randomCode,
                Expiration = DateTime.UtcNow.AddMinutes(5) 
            };

            _context.passwordResetCodes.Add(passwordResetCode);
            await _context.SaveChangesAsync();

            // Gửi mã qua email
            await _emailSender.SendEmailAsync(user.Email, "Reset Password Code", $"Your password reset code is: {randomCode}");

            return Ok("Password reset code sent. Please check your email.");
        }
        [HttpPost]
        [Route("ResetPasswordWithCode")]
        public async Task<IActionResult> ResetPasswordWithCode([FromBody] ResetPasswordRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }

            var resetCodeRecord = await _context.passwordResetCodes
                .FirstOrDefaultAsync(x => x.Email == model.Email && x.Code == model.Token && x.IsUsed == false);

            if (resetCodeRecord == null || resetCodeRecord.Expiration < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired reset code.");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), model.NewPassword);
            if (result.Succeeded)
            {
                // Đánh dấu mã xác thực đã được sử dụng
                resetCodeRecord.IsUsed = true;
                _context.passwordResetCodes.Update(resetCodeRecord);
                await _context.SaveChangesAsync();

                return Ok("Password has been reset successfully.");
            }

            return BadRequest("You entered the wrong code or provided incorrect information!");
        }
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // Implement any server-side logout logic if necessary
            // Example: Log the logout event
            // Example: Invalidate the JWT token if you're storing tokens server-side
            return NoContent(); // Or return any appropriate response
        }
    }
}
