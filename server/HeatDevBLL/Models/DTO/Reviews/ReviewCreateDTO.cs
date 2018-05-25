using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HeatDevBLL.Models.DTO
{
    public class ReviewCreateDTO
    {
        [JsonProperty("order_id")]
        [Required, Range(0, int.MaxValue)]
        public int Id { get; set; }

        [JsonProperty("text")]
        [Required, StringLength(1500, MinimumLength = 10)]
        public string Text { get; set; }

        [JsonProperty("rating")]
        [Required, Range(1, 5)]
        public int Rating { get; set; }
    }
}
