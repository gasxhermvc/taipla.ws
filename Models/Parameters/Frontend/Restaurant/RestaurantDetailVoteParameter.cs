using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Restaurant
{
    public class RestaurantDetailVoteParameter : BaseFrontendParameter
    {

        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "restaurantId is required.")]
        [BindProperty(Name = "restaurantId", SupportsGet = true)]
        public int RESTAURANT_ID { get; set; }

        [FromBody]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [BindProperty(Name = "deviceId", SupportsGet = true)]
        [JsonProperty("deviceId")]
        public string DEVICE_ID { get; set; }

        [FromBody]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "score is required.")]
        [Range(1, 5, ErrorMessage = "score input range is 1 to 5")]
        [BindProperty(Name = "score", SupportsGet = true)]
        [JsonProperty("score")]
        public VoteScoreEnum SCORE { get; set; }
    }
}
