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
    public class AccountProfileParameter : BaseFrontendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "email is required.")]
        [EmailAddress(ErrorMessage = "email is email address")]
        [BindProperty(Name = "email", SupportsGet = true)]
        public string EMAIL { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "phoneNumber is required.")]
        [RegularExpression(ValidateExtension.PHONE_NUMBER_THAI, ErrorMessage = ValidateExtension.PHONE_NUMBER_THAI_ERR_MSG)]
        [BindProperty(Name = "phoneNumber", SupportsGet = true)]
        public string PHONE_NUMBER { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "firstName is required.")]
        [BindProperty(Name = "firstName", SupportsGet = true)]
        public string FIRST_NAME { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "lastName is required.")]
        [BindProperty(Name = "lastName", SupportsGet = true)]
        public string LAST_NAME { get; set; }

        [BindProperty(Name = "avatarImage", SupportsGet = true)]
        public IFormFile AVATAR_IMAGE { get; set; }

    }
}
