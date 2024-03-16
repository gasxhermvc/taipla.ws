using Microsoft.AspNetCore.Http;
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
    public class RestaurantDetailReviewParameter : BaseFrontendParameter
    {
        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "restaurantId is required.")]
        [BindProperty(Name = "restaurantId", SupportsGet = true)]
        public int RESTAURANT_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "comment is required.")]
        [BindProperty(Name = "comment", SupportsGet = true)]
        public string COMMENT { get; set; }

        [BindProperty(Name = "images", SupportsGet = true)]
        public IFormFile IMAGES { get; set; }
    }
}
