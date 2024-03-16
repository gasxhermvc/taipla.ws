using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Restaurant
{
    public class RestaurantPromotionParameter : BaseFrontendParameter
    {
        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "restaurantId is required.")]
        [BindProperty(Name = "restaurantId", SupportsGet = true)]
        public int RESTAURANT_ID { get; set; }


        //[BindRequired]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        //[BindProperty(Name = "deviceId", SupportsGet = true)]
        //public string DEVICE_ID { get; set; }
    }
}
