using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HeatDevBLL.Models.DTO
{
    public class UserSignInDTO
    {
        [JsonProperty("login")]
        [Required]
        public string Login { get; set; }

        [JsonProperty("password")]
        [Required]
        public string Password { get; set; }
    }
}
