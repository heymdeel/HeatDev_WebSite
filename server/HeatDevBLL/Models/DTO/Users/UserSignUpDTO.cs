using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HeatDevBLL.Models.DTO
{
    public class UserSignUpDTO
    {
        [Required, StringLength(15, MinimumLength = 3)]
        public string Login { get; set; }

        [Required, StringLength(20, MinimumLength = 5)]
        public string Password { get; set; }

        public string Avatar { get; set; }

        [Required, StringLength(15, MinimumLength = 3)]
        public string Name { get; set; }

        [Required, StringLength(15, MinimumLength = 3)]
        public string Surname { get; set; }

        [RegularExpression("^[7][0-9]{10}$")]
        public string Phone { get; set; }
    }
}
