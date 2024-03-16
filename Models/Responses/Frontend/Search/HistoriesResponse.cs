using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Parameters.Frontend.Search;

namespace Taipla.Webservice.Models.Responses.Frontend.Search
{
    public class HistoriesResponse
    {
        [JsonProperty("searchId")]
        public int Id { get; set; }
        [JsonProperty("lebel")]
        public string SearchText { get; set; }
        [JsonProperty("condition")]
        public SearchConditionParameter Condition { get; set; }
        [JsonProperty("hashing")]
        public string Hashing { get; set; }
        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }
    }
}
