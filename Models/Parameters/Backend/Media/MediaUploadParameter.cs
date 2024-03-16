using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;

namespace Taipla.Webservice.Models.Parameters.Backend.Media
{
    public class MediaUploadParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PATH is required.")]
        [BindProperty(Name = "PATH", SupportsGet = true)]
        public UploadEnum SystemName { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "REF_ID is required.")]
        [BindProperty(Name = "REF_ID", SupportsGet = true)]
        public string RefId { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "UPLOAD is required.")]
        [BindProperty(Name = "UPLOAD", SupportsGet = true)]
        public IFormCollection Uploads { get; set; }
    }
}
