using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend
{
    public class MenuResponse
    {
        [JsonProperty("Id")]
        public int MENU_ID_1 { get; set; }
        [JsonProperty("menuId")]
        public int MENU_ID_2 { get; set; }
        [JsonProperty("restaurantId")]
        public int RESTAURANT_ID { get; set; }
        [JsonProperty("name")]
        public string NAME { get; set; }
        [JsonProperty("price")]
        public double PRICE { get; set; }
        [JsonProperty("image")]
        public string IMAGE { get; set; }
        [JsonProperty("imageSM")]
        public string IMAGE_SM { get; set; }
        [JsonProperty("imageMD")]
        public string IMAGE_MD { get; set; }
        [JsonProperty("imageLG")]
        public string IMAGE_LG { get; set; }
        [JsonProperty("unit")]
        public string UNIT { get; set; }
        [JsonProperty("viewer")]
        public int VIEWER { get; set; }
    }
}
