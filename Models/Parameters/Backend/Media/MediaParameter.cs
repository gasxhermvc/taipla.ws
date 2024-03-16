using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;

namespace Taipla.Webservice.Models.Parameters.Backend.Media
{
    public class MediaParameter
    {
        //[BindProperty(Name = "REF_ID", SupportsGet = true)]
        //public string REF_ID { get; set; }


        [BindProperty(Name = "PATH", SupportsGet = true)]
        public UploadEnum? PATH { get; set; }
    }
}
