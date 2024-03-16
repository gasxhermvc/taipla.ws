using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Um
{
    public class UmUpdatedParameter : BaseBackendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "USER_ID is required.")]
        public long USER_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "USERNAME is required.")]
        public string USERNAME { get; set; }

        public string FIRST_NAME { get; set; }

        public string LAST_NAME { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PHONE is required.")]
        public string PHONE { get; set; }

        [BindRequired]
        [EmailAddress(ErrorMessage = "Format is email address.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "EMAIL is required.")]
        public string EMAIL { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "ROLE is required.")]
        public string ROLE { get; set; }

        public string CLINET_ID { get; set; }

        public IFormFile UPLOAD { get; set; }
    }
}
