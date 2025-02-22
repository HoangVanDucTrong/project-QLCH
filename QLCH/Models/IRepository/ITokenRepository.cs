using Microsoft.AspNetCore.Identity;

namespace QLCH.Models.IRepository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles, int storeId);
        string CreateJWTToken2(IdentityUser user, List<string> roles, int storeId);
    }


}
