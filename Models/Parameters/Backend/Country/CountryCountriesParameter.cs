using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Country
{
    public class CountryCountriesParameter : BaseBackendParameter
    {
        [FromRoute]
        [ValueFilter]
        public int? COUNTRY_ID { get; set; }
    }
}
