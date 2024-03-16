using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Parameters.Backend.Auth
{
    public class LoginParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "username is required.")]
        [JsonProperty("username")]
        public string USERNAME { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "password is required.")]
        [JsonProperty("password")]
        public string PASSWORD { get; set; }

        [JsonProperty("remember_me")]
        public bool? REMEMBER_ME { get; set; }
    }
}
