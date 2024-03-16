using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Culture
{
    public class CultureCreatedParameter : BaseBackendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "COUNTRY_ID is required.")]
        public int? COUNTRY_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "NAME_TH is required.")]
        public string NAME_TH { get; set; }

        public string NAME_EN { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "DESCRIPTION is required.")]
        public string DESCRIPTION { get; set; }

        public IFormFile UPLOAD { get; set; }
    }
}
