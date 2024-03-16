using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend.FirstPage
{
    public class CulturesResponse
    {
        [JsonProperty("country")]
        public string COUNTRY { get; set; }
        [JsonProperty("culture")]
        public string CULTURE { get; set; }
        [JsonProperty("foods")]
        public List<FoodResponse> FOODS { get; set; }
    }
}
