using Microsoft.AspNetCore.Identity;

namespace QLCH.Models
{
    public class AuthorcationStore : IdentityUser
    {
   
        public string Sdt { get; set; }
        public string DiaChi { get; set; }
        public string QuocGia { get; set; }
        

    }
}
