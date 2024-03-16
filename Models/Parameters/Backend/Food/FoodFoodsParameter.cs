using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Food
{
    public class FoodFoodsParameter : BaseBackendParameter
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
        [BindProperty(Name = "NAME_TH", SupportsGet = true)]
        public string NAME_TH { get; set; }

        [FromRoute]
        [ValueFilter]
        [BindProperty(Name = "FOOD_ID", SupportsGet = true)]
        public int? FOOD_ID { get; set; }


        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "AUTHOR", SupportsGet = true)]
        public int? AUTHOR { get; set; }
    }
}
