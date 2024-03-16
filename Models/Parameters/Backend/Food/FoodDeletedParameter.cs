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
    public class FoodDeletedParameter : BaseBackendParameter
    {
        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "COUNTRY_ID is required.")]
        public int? COUNTRY_ID { get; set; }

        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "CULTURE_ID is required.")]
        public int? CULTURE_ID { get; set; }

        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "FOOD_ID is required.")]
        public int? FOOD_ID { get; set; }
    }
}
