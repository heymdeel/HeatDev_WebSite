using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatDev.ViewModels
{
    public class ReviewVM
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; } 

        [JsonProperty("client_profile")]
        public UserReviewVM UserProfile { get; set; }
    }
}
