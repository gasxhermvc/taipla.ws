using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.RestaurantMenu
{
    public class RestaurantMenuDeletedParameter : BaseBackendParameter
    {
        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "COUNTRY_ID is required.")]
        [BindProperty(Name = "COUNTRY_ID", SupportsGet = true)]
        public int? COUNTRY_ID { get; set; }

        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "CULTURE_ID is required.")]
        [BindProperty(Name = "CULTURE_ID", SupportsGet = true)]
        public int? CULTURE_ID { get; set; }

        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "RES_ID is required.")]
        [BindProperty(Name = "RES_ID", SupportsGet = true)]
        public int? RES_ID { get; set; }

        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "MENU_ID is required.")]
        [BindProperty(Name = "MENU_ID", SupportsGet = true)]
        public int? MENU_ID { get; set; }
    }
}
