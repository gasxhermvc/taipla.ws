using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.FirstPage
{
    public class FirstPageFoodCategoriesParameter : BaseFrontendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "countryId is required.")]
        [BindProperty(Name = "countryId")]
        public int COUNTRY_ID { get; set; }

        //[BindRequired]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "cultureId is required.")]
        [BindProperty(Name = "cultureId")]
        public int CULTURE_ID { get; set; } = -1;

    }
}
