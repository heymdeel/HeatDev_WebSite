using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HeatDevBLL.Models.DTO
{
    public class UserSignUpDTO
    {
        [JsonProperty("login")]
        [Required, StringLength(15, MinimumLength = 3)]
        public string Login { get; set; }

        [JsonProperty("password")]
        [Required, StringLength(20, MinimumLength = 5)]
        public string Password { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("name")]
        [Required, StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        [JsonProperty("surname")]
        [Required, StringLength(15, MinimumLength = 3)]
        public string Surname { get; set; }

        [JsonProperty("phone")]
        [RegularExpression("^[7][0-9]{10}$")]
        public string Phone { get; set; }
    }
}
