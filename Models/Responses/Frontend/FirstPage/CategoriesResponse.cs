using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend.FirstPage
{
    public class CategoriesResponse
    {
        [JsonProperty("country")]
        public string COUNTRY { get; set; }
        [JsonProperty("foods")]
        public List<CategoriesCulturesResponse> FOODS { get; set; } = new List<CategoriesCulturesResponse>();
    }

    public class CategoriesCulturesResponse
    {
        [JsonProperty("total")]
        public int TOTAL { get; set; }
        [JsonProperty("culture")]
        public string CULTURE { get; set; }
        [JsonProperty("foodCulture")]
        public List<FoodResponse> FOOD_CULTURE { get; set; }
    }
}
