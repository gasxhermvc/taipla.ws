using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Bases
{
    public class BaseFrontendParameter
    {
        [FromQuery]
        public int limit { get; set; } = 10;
    }

    public class BaseBackendParameter
    {
        public virtual int limit { get; set; } = 10;
    }

    public class BaseUploaFrontenddParameter : BaseFrontendParameter
    {
        public string fileName { get; set; }

        public virtual IFormFile upload { get; set; }
    }
}
