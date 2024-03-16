using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Restaurant
{
    public class RestaurantRestaurantsParameter : BaseBackendParameter
    {
        [FromRoute]
        [ValueFilter]
        [BindProperty(Name = "RES_ID", SupportsGet = true)]
        public int? RES_ID { get; set; }

        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "COUNTRY_ID", SupportsGet = true)]
        public int? COUNTRY_ID { get; set; }

        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "OWNER_ID", SupportsGet = true)]
        public int? OWNER_ID { get; set; }

        [FromQuery]
        [BindProperty(Name = "NAME", SupportsGet = true)]
        public string NAME { get; set; }

        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "AUTHOR", SupportsGet = true)]
        public int? AUTHOR { get; set; }
    }
}
