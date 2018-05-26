using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HeatDevBLL.Models.DTO
{
    public class ChangePasswordDTO
    {
        [JsonProperty("old_password")]
        [Required, StringLength(20, MinimumLength = 5)]
        public string OldPassword { get; set; }

        [JsonProperty("new_password")]
        [Required, StringLength(20, MinimumLength = 5)]
        public string NewPassword { get; set; }
    }
}
