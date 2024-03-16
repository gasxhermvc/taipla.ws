using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend
{
    public class LegendResponse
    {
        [JsonProperty("title")]
        public string TITLE { get; set; }
        [JsonProperty("legend")]
        public List<Legend> LEGEND { get; set; }
    }

    public class Legend
    {
        [JsonProperty("legendId")]
        public int LEGEND_ID { get; set; }
        [JsonProperty("name")]
        public string NAME { get; set; }
        [JsonProperty("description")]
        public string DESCRIPTION { get; set; }
        [JsonProperty("image")]
        public string IMAGE { get; set; }
        [JsonProperty("imageSM")]
        public string IMAGE_SM { get; set; }
        [JsonProperty("imageMD")]
        public string IMAGE_MD { get; set; }
        [JsonProperty("imageLG")]
        public string IMAGE_LG { get; set; }
    }
}
