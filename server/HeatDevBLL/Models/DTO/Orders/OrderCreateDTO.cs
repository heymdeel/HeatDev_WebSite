using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HeatDevBLL.Models.DTO
{
    public class OrderCreateDTO
    {
        [JsonProperty("category")]
        [Required]
        public int CategoryId { get; set; }

        [JsonProperty("address")]
        [Required, StringLength(75, MinimumLength = 5)]
        public string Address { get; set; }

        [JsonProperty("visit_date")]
        [Required]
        public DateTime VisitTime { get; set; }
    }
}
