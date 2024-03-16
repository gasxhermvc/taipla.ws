using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Account
{
    public class ChangePasswordParameter : BaseFrontendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "passwordOld is required.")]
        [BindProperty(Name = "passwordOld", SupportsGet = true)]
        [JsonProperty("passwordOld")]
        public string PASSWORD_OLD { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "passwordNew is required.")]
        [BindProperty(Name = "passwordNew", SupportsGet = true)]
        [JsonProperty("passwordNew")]
        public string PASSWORD_NEW { get; set; }

    }
}
