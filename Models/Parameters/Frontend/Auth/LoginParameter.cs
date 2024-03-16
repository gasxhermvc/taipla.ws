using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Auth
{
    public class LoginParameter : BaseFrontendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "username is required.")]
        [BindProperty(Name = "username", SupportsGet = true)]
        [JsonProperty("username")]
        public string USERNAME { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "password is required.")]
        [BindProperty(Name = "password", SupportsGet = true)]
        [JsonProperty("password")]
        public string PASSWORD { get; set; }

        [BindProperty(Name = "remember_me", SupportsGet = true)]
        [JsonProperty("rememberMe")]
        public bool? REMEMBER_ME { get; set; } = true;

        [BindProperty(Name = "deviceId", SupportsGet = true)]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [JsonProperty("deviceId")]
        public string DEVICE_NAME { get; set; }
    }
}
