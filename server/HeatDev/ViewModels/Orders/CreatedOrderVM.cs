using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatDev.ViewModels
{
    public class CreatedOrderVM
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("client_id")]
        public int ClientId { get; set; }

        [JsonProperty("beginning_time")]
        public DateTime BeginningTime { get; set; }

        [JsonProperty("category")]
        public int CategoryId { get; set; }

        [JsonProperty("status")]
        public int StatusId { get; set; }

        [JsonProperty("diagnostic_price")]
        public double DiagnosticPrice { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("visit_time")]
        public DateTime VisitTime { get; set; }
    }
}
