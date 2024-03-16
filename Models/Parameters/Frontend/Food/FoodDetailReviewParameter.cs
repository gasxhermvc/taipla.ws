using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Food
{
    public class FoodDetailReviewParameter : BaseFrontendParameter
    {
        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "foodId is required.")]
        [BindProperty(Name = "foodId", SupportsGet = true)]
        public int foodId { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "comment is required.")]
        [BindProperty(Name = "comment", SupportsGet = true)]
        public string comment { get; set; }

        [BindProperty(Name = "images", SupportsGet = true)]
        public IFormFile images { get; set; }
    }
}
