using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.RestaurantMenu
{
    public class RestaurantMenuRestaurantMenusParameter : BaseBackendParameter
    {
        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "COUNTRY_ID", SupportsGet = true)]
        public int? COUNTRY_ID { get; set; }

        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "CULTURE_ID", SupportsGet = true)]
        public int? CULTURE_ID { get; set; }

        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "OWNER_ID", SupportsGet = true)]
        public int? OWNER_ID { get; set; }

        [FromQuery]
        [Required(AllowEmptyStrings = false, ErrorMessage = "RES_ID is required.")]
        [BindProperty(Name = "RES_ID", SupportsGet = true)]
        public int? RES_ID { get; set; }

        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "MENU_ID", SupportsGet = true)]
        public int? MENU_ID { get; set; }
    }
}
