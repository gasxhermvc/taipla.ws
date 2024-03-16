using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Parameters.Frontend.Auth
{
    public class LogoutParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "clientId is required.")]
        [BindProperty(Name = "clientId", SupportsGet = true)]
        [JsonProperty("clientId")]
        public string CLIENT_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [BindProperty(Name = "deviceId", SupportsGet = true)]
        [JsonProperty("deviceId")]
        public string DEVICE_ID { get; set; }
    }
}
