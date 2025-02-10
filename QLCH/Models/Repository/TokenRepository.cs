using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QLCH.Models.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QLCH.Models.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;
        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateJWTToken(IdentityUser user, List<string> roles, int storeId)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, user.Email),
       new Claim("StoreId", storeId.ToString())

    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:JwtSecurityKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
                claims:claims,
             expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);
      
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string CreateJWTToken2(IdentityUser user, List<string> roles, int storeId)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName), // Thay đổi từ ClaimTypes.Email thành ClaimTypes.Name
        new Claim("StoreId", storeId.ToString())
    };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:JwtSecurityKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
