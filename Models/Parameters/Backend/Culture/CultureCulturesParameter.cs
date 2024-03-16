using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Culture
{
    public class CultureCulturesParameter : BaseBackendParameter
    {
        [FromRoute]
        [ValueFilter]
        public int? COUNTRY_ID { get; set; }

        [FromRoute]
        [ValueFilter]
        public int? CULTURE_ID { get; set; }
    }
}
