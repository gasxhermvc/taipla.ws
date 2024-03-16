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
    public class MediaPreviewParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "MEDIA_ID is required.")]
        [BindProperty(Name = "MEDIA_ID", SupportsGet = true)]
        public int MEDIA_ID { get; set; }

        [BindProperty(Name = "REF_ID", SupportsGet = true)]
        public string REF_ID { get; set; }


        [BindProperty(Name = "PATH", SupportsGet = true)]
        public UploadEnum PATH { get; set; }
    }
}
