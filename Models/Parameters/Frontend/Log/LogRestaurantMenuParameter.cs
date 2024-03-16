using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Log
{
    public class LogRestaurantMenuParameter : BaseFrontendParameter
    {
        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "restaurantId is required.")]
        [BindProperty(Name = "restaurantId", SupportsGet = true)]
        public int restaurantId { get; set; }

        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "menuId is required.")]
        [BindProperty(Name = "menuId", SupportsGet = true)]
        public int menuId { get; set; }

        [FromBody]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [BindProperty(Name = "deviceId", SupportsGet = true)]
        public string deviceId { get; set; }
    }
}
