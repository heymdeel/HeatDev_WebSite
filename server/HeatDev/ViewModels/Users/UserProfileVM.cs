using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatDev.ViewModels
{
    public class UserProfileVM
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; } 

        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }
}
