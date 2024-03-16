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
    public class MediaRemoveUploadParameter
    {
        [FromRoute]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "UID is required.")]
        [BindProperty(Name = "UID", SupportsGet = true)]
        public string UID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PATH is required.")]
        [BindProperty(Name = "PATH", SupportsGet = true)]
        public UploadEnum Path { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PATH_FILE is required.")]
        [BindProperty(Name = "PATH_FILE", SupportsGet = true)]
        public string PathFile { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "REF_ID is required.")]
        [BindProperty(Name = "REF_ID", SupportsGet = true)]
        public string RefId { get; set; }

        [BindProperty(Name = "SYSTEM_NAME", SupportsGet = true)]
        public string SystemName { get; set; }
    }
}
