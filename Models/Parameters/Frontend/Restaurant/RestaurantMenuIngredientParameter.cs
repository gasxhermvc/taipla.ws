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
    public class RestaurantMenuIngredientParameter : BaseFrontendParameter
    {
        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "restaurantId is required.")]
        [BindProperty(Name = "restaurantId", SupportsGet = true)]
        public int RESTAURANT_ID { get; set; }

        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "menuId is required.")]
        [BindProperty(Name = "menuId", SupportsGet = true)]
        public int MENU_ID { get; set; }

    }
}
