using System.Security.Claims;

namespace QLCH.Models.IRepository
{
    public interface IGetClaimsFromToken
    {
        ClaimsPrincipal laytoken(string token);
    }
}
