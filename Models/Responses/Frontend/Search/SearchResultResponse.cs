using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend.Search
{
    public class SearchResultResponse
    {
        [JsonProperty("title")]
        public string TITLE { get; set; }
        [JsonProperty("foodResult")]
        public List<FoodResponse> foodResult { get; set; }
        [JsonProperty("restaurantResult")]
        public List<RestaurantResponse> restaurantResult { get; set; }
    }
}
