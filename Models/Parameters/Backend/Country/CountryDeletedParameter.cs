using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Country
{
    public class CountryDeletedParameter : BaseBackendParameter
    {
        [ValueFilter]
        [Required(AllowEmptyStrings = false, ErrorMessage = "COUNTRY_ID is required.")]
        public int? COUNTRY_ID { get; set; }
    }
}
