<<<<<<< HEAD
﻿using QLCH.Models.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QLCH.Models.Repository
{
    public class GetClaimsFromToken : IGetClaimsFromToken
    {
        public ClaimsPrincipal laytoken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                throw new ArgumentException("Invalid token");

            var claims = jwtToken.Claims;
            var claimsIdentity = new ClaimsIdentity(claims);

            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
=======
﻿using QLCH.Models.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QLCH.Models.Repository
{
    public class GetClaimsFromToken : IGetClaimsFromToken
    {
        public ClaimsPrincipal laytoken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                throw new ArgumentException("Invalid token");

            var claims = jwtToken.Claims;
            var claimsIdentity = new ClaimsIdentity(claims);

            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
