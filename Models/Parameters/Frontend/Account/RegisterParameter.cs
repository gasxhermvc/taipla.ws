using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Account
{
    public class RegisterParameter : BaseFrontendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "email is required.")]
        [EmailAddress(ErrorMessage = "email is email address.")]
        [BindProperty(Name = "email", SupportsGet = true)]
        public string EMAIL { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "username is required.")]
        [BindProperty(Name = "username", SupportsGet = true)]
        public string USERNAME { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "password is required.")]
        [BindProperty(Name = "password", SupportsGet = true)]
        public string PASSWORD { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "phoneNumber is required.")]
        [RegularExpression(ValidateExtension.PHONE_NUMBER_THAI, ErrorMessage = ValidateExtension.PHONE_NUMBER_THAI_ERR_MSG)]
        [BindProperty(Name = "phoneNumber", SupportsGet = true)]
        public string PHONE_NUMBER { get; set; }

        [BindProperty(Name = "avatarImage", SupportsGet = true)]
        public IFormFile AVATAR_IMAGE { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [BindProperty(Name = "deviceId", SupportsGet = true)]
        public string DEVICE_ID { get; set; } = Guid.NewGuid().ToString();
    }
}
