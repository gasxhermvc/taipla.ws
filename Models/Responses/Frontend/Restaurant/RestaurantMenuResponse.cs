using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend.Restaurant
{
    public class RestaurantMenuResponse
    {
        [JsonProperty("title")]
        public string TITLE { get; set; }
        [JsonProperty("menu")]
        public List<MenuResponse> MENU { get; set; } = new List<MenuResponse>();
    }
}
