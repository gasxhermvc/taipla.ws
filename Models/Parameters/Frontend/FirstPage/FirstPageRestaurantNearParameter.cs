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
    public class FirstPageRestaurantNearParameter : BaseFrontendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [BindProperty(Name = "deviceId", SupportsGet = true)]
        public string deviceId { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "croodinates is required.")]
        [BindProperty(Name = "croodinates", SupportsGet = true)]
        public List<string> croodinates { get; set; }

        [BindProperty(Name = "distance", SupportsGet = true)]
        public int distance { get; set; } = 20000;
    }
}
