using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend
{
    public class RestaurantDetailResponse
    {
        [JsonProperty("Id")]
        public int RESTAURANT_ID_S1 { get; set; }
        [JsonProperty("restaurantId")]
        public int RESTAURANT_ID_S2 { get; set; }
        [JsonProperty("countryId")]
        public int COUNTRY_ID { get; set; }
        [JsonProperty("name")]
        public string NAME { get; set; }
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
        [JsonProperty("rating")]
        public double RATING { get; set; }
        [JsonProperty("coordinates")]
        public List<double> COORDINATES { get; set; }
        [JsonProperty("address")]
        public string ADDRESS { get; set; }
        [JsonProperty("phone")]
        public string PHONE { get; set; }
        [JsonProperty("website")]
        public string WEBSITE { get; set; }
        [JsonProperty("facebook")]
        public string FACEBOOK { get; set; }
        [JsonProperty("line")]
        public string LINE { get; set; }
        [JsonProperty("video")]
        public string VIDEO { get; set; }
        [JsonProperty("openTime")]
        public string OPEN_TIME { get; set; }
        [JsonProperty("isCarPark")]
        public bool IS_CAR_PARK { get; set; }
        [JsonProperty("owner")]
        public string OWNER { get; set; }

    }
}
