using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Parameters.Frontend.Log
{
    public class LogRestaurantPromotionParameter
    {
        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "restaurantId is required.")]
        [BindProperty(Name = "restaurantId", SupportsGet = true)]
        public int restaurantId { get; set; }

        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "promotionId is required.")]
        [BindProperty(Name = "promotionId", SupportsGet = true)]
        public int promotionId { get; set; }

        [FromBody]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [BindProperty(Name = "deviceId", SupportsGet = true)]
        public string deviceId { get; set; }
    }
}
