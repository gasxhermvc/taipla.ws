using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend
{
    public class ImageListResponse
    {
        [JsonProperty("image")]
        public List<string> IMAGE { get; set; } = new List<string>();
        [JsonProperty("imageSM")]
        public List<string> IMAGE_SM { get; set; } = new List<string>();
        [JsonProperty("imageMD")]
        public List<string> IMAGE_MD { get; set; } = new List<string>();
        [JsonProperty("imageLG")]
        public List<string> IMAGE_LG { get; set; } = new List<string>();
    }
}
