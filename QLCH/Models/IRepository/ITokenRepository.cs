<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Identity;

namespace QLCH.Models.IRepository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles, int storeId);
        string CreateJWTToken2(IdentityUser user, List<string> roles, int storeId);
    }


}
=======
﻿using Microsoft.AspNetCore.Identity;

namespace QLCH.Models.IRepository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles, int storeId);
        string CreateJWTToken2(IdentityUser user, List<string> roles, int storeId);
    }


}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
