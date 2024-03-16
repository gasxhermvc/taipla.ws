using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Um
{
    public class UmChangePasswordParameter : BaseBackendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "USER_ID is required.")]
        public long USER_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PASSWORD_OLD is required.")]
        public string PASSWORD_OLD { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PASSWORD_NEW is required.")]
        public string PASSWORD_NEW { get; set; }
    }
}
