using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend
{
    public class RestaurantResponse
    {
        [JsonProperty("Id")]
        public int RES_ID_S1 { get; set; }
        [JsonProperty("resId")]
        public int RES_ID_S2 { get; set; }
        [JsonProperty("countryId")]
        public int COUNTRY_ID { get; set; }
        [JsonProperty("name")]
        public string NAME { get; set; }
        [JsonProperty("image")]
        public string IMAGE { get; set; }
        [JsonProperty("imageSM")]
        public string IMAGE_SM { get; set; }
        [JsonProperty("imageMD")]
        public string IMAGE_MD { get; set; }
        [JsonProperty("imageLG")]
        public string IMAGE_LG { get; set; }
        [JsonProperty("viewer")]
        public int VIEWER { get; set; }

    }
}
