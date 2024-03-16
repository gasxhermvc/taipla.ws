using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Parameters.Frontend.Restaurant
{
    public class RestaurantMenuDetailParameter : RestaurantParameter
    {
        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "MENU_ID is required.")]
        [BindProperty(Name = "menuId", SupportsGet = true)]
        public int MENU_ID { get; set; }
    }
}
