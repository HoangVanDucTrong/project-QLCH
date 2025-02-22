<<<<<<< HEAD
﻿using System.Text.Json.Serialization;

namespace QLCH_MVC.Models.LoginDTO
{
    public class LoginResponseDTO
    {
        [JsonPropertyName("jwtToken")]
        public string JwtToken { get; set; }
        public List<string> Roles { get; set; }
    }

}
=======
﻿using System.Text.Json.Serialization;

namespace QLCH_MVC.Models.LoginDTO
{
    public class LoginResponseDTO
    {
        [JsonPropertyName("jwtToken")]
        public string JwtToken { get; set; }
        public List<string> Roles { get; set; }
    }

}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
