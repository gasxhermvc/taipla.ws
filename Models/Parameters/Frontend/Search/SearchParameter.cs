using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Search
{
    public class SearchParameter : BaseFrontendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [BindProperty(Name = "deviceId", SupportsGet = true)]
        [JsonProperty("deviceId")]
        public string DEVICE_ID { get; set; }

        [JsonProperty("text")]
        [BindProperty(Name = "text", SupportsGet = true)]
        public string TEXT { get; set; }

        [JsonProperty("condition")]
        [BindProperty(Name = "condition", SupportsGet = true)]
        public SearchConditionParameter condition { get; set; } = new SearchConditionParameter();
    }

    public class SearchConditionParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "direction is required.")]
        [BindProperty(Name = "direction", SupportsGet = true)]
        [JsonProperty("direction")]
        public DirectionEnum DIRECTION { get; set; } = DirectionEnum.ALL;

        [BindProperty(Name = "countryId", SupportsGet = true)]
        [JsonProperty("countryId")]
        [ValueFilter]
        public int? COUNTRY_ID { get; set; }

        [BindProperty(Name = "cultureId", SupportsGet = true)]
        [JsonProperty("cultureId")]
        [ValueFilter]
        public int? CULTURE_ID { get; set; }

        [BindProperty(Name = "tags", SupportsGet = true)]
        [JsonProperty("tags")]
        public List<string> TAGS { get; set; } = new List<string>();
    }
}
