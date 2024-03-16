using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend.FirstPage
{
    public class RestaurantNearResponse
    {
        [JsonProperty("title")]
        public string TITLE { get; set; }

        [JsonProperty("restaurants")]
        public List<NearResponse> RESTAURANTS { get; set; } = new List<NearResponse>();
    }
}
