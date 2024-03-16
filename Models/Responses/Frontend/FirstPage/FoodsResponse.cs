using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend.FirstPage
{
    public class FoodsResponse
    {
        [JsonProperty("total")]
        public int TOTAL { get; set; }
        [JsonProperty("country")]
        public string COUNTRY_NAME { get; set; }

        [JsonProperty("foods")]
        public List<FoodResponse> FOODS { get; set; } = new List<FoodResponse>();
    }
}
