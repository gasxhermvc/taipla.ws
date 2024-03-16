using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Um
{
    public class UmUserParameter : BaseBackendParameter
    {
        [FromRoute]
        [ValueFilter]
        public int? USER_ID { get; set; }
    }
}
