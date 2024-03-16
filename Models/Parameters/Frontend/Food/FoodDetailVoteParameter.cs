using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Food
{
    public class FoodDetailVoteParameter : BaseFrontendParameter
    {
        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "foodId is required.")]
        [BindProperty(Name = "foodId", SupportsGet = true)]
        public int foodId { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "score is required.")]
        [Range(1, 5, ErrorMessage = "score input range is 1 to 5")]
        [BindProperty(Name = "score", SupportsGet = true)]
        public VoteScoreEnum score { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [BindProperty(Name = "deviceId", SupportsGet = true)]
        public string deviceId { get; set; }

    }
}
