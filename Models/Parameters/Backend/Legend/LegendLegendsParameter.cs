using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Legend
{
    public class LegendLegendsParameter : BaseBackendParameter
    {

        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "LEGEND_ID", SupportsGet = true)]
        public int? LEGEND_ID { get; set; }


        [FromRoute]
        [FromQuery]
        [BindProperty(Name = "CODE", SupportsGet = true)]
        public string CODE { get; set; }
    }
}
