using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.FirstPage
{
    public class FirstPageFoodTopViewerParameter : BaseFrontendParameter
    {
        [BindProperty(Name = "rand")]
        public bool rand { get; set; } = true;
    }
}
