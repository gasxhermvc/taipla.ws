using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend
{
    public class RestaurantMenuDetailResponse
    {
        [JsonProperty("Id")]
        public int MENU_ID_S1 { get; set; }
        [JsonProperty("memuId")]
        public int MENU_ID_S2 { get; set; }
        [JsonProperty("restaurantId")]
        public int RESTAURANT_ID { get; set; }
        [JsonProperty("countryId")]
        public int COUNTRY_ID { get; set; }
        [JsonProperty("cultureId")]
        public int CULTURE_ID { get; set; }
        [JsonProperty("name")]
        public string NAME { get; set; }
        [JsonProperty("cookingFood")]
        public string COOKING_FOOD { get; set; }
        [JsonProperty("image")]
        public List<string> IMAGE { get; set; } = new List<string>();
        [JsonProperty("imageSM")]
        public List<string> IMAGE_SM { get; set; } = new List<string>();
        [JsonProperty("imageMD")]
        public List<string> IMAGE_MD { get; set; } = new List<string>();
        [JsonProperty("imageLG")]
        public List<string> IMAGE_LG { get; set; } = new List<string>();
        [JsonProperty("viewer")]
        public int VIEWER { get; set; }
        [JsonProperty("legendStatus")]
        public bool LEGEND_STATUS { get; set; }
        [JsonProperty("price")]
        public double PRICE { get; set; }
        [JsonProperty("unit")]
        public string UNIT { get; set; }
        [JsonProperty("rating")]
        public double RATING { get; set; }
    }
}
