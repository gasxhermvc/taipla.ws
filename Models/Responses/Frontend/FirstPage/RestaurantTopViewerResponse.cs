using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend.FirstPage
{
    public class RestaurantTopViewerResponse
    {
        [JsonProperty("title")]
        public string TITLE { get; set; }

        [JsonProperty("restaurants")]
        public List<RestaurantResponse> RESTAURANTS { get; set; } = new List<RestaurantResponse>();
    }
}
