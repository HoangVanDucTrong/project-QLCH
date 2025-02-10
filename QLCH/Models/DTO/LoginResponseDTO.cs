namespace QLCH.Models.DTO
{
    public class LoginResponseDTO
    {
        public string JwtToken { set; get; }
        public List<string> Roles { get; set; }
    }
}
