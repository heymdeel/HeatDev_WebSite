using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatDev.ViewModels
{
    public class ClientOrdersVM
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("beginning_time")]
        public DateTime BeginningTime { get; set; }

        [JsonProperty("visit_time")]
        public DateTime VisitTime { get; set; }

        [JsonProperty("category")]
        public int CategoryId { get; set; }

        [JsonProperty("status")]
        public int StatusId { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
