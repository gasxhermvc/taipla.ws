using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend.FirstPage
{
    public class FoodTopViewerResponse
    {
        [JsonProperty("title")]
        public string TITLE { get; set; }

        [JsonProperty("foods")]
        public List<FoodResponse> FOODS { get; set; } = new List<FoodResponse>();
    }
}
