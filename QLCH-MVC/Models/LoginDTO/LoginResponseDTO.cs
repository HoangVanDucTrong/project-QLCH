using System.Text.Json.Serialization;

namespace QLCH_MVC.Models.LoginDTO
{
    public class LoginResponseDTO
    {
        [JsonPropertyName("jwtToken")]
        public string JwtToken { get; set; }
        public List<string> Roles { get; set; }
    }

}
