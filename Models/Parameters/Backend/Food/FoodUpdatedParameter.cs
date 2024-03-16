using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Food
{
    public class FoodUpdatedParameter : BaseBackendParameter
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
        [Required(AllowEmptyStrings = false, ErrorMessage = "FOOD_ID is required.")]
        [BindProperty(Name = "FOOD_ID", SupportsGet = true)]
        public int? FOOD_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "NAME_TH is required.")]
        [BindProperty(Name = "NAME_TH", SupportsGet = true)]
        public string NAME_TH { get; set; }

        [BindProperty(Name = "NAME_EN", SupportsGet = true)]
        public string NAME_EN { get; set; }

        [BindProperty(Name = "DESCRIPTION", SupportsGet = true)]
        public string DESCRIPTION { get; set; }

        [BindProperty(Name = "COOKING_FOOD", SupportsGet = true)]
        public string COOKING_FOOD { get; set; }

        [BindProperty(Name = "DIETETIC_FOOD", SupportsGet = true)]
        public string DIETETIC_FOOD { get; set; }

        [BindProperty(Name = "INGREDIENT", SupportsGet = true)]
        public string INGREDIENT { get; set; }

        [BindProperty(Name = "LEGEND", SupportsGet = true)]
        public string LEGEND { get; set; }

        [BindProperty(Name = "UPLOAD", SupportsGet = true)]
        public IFormFile UPLOAD { get; set; }
    }
}
