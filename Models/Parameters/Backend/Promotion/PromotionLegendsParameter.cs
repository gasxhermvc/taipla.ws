using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Promotion
{
    public class PromotionPromotionsParameter : BaseBackendParameter
    {
        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "RES_ID", SupportsGet = true)]
        public int? RES_ID { get; set; }

        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "PROMOTION_ID", SupportsGet = true)]
        public int? PROMOTION_ID { get; set; }

        [FromQuery]
        [ValueFilter]
        [BindProperty(Name = "PROMOTION_TYPE", SupportsGet = true)]
        public int? PROMOTION_TYPE { get; set; }

        [DataType(DataType.Date)]
        [BindProperty(Name = "START_DATE", SupportsGet = true)]
        public DateTime? START_DATE { get; set; }

        [DataType(DataType.Date)]
        [BindProperty(Name = "END_DATE", SupportsGet = true)]
        public DateTime? END_DATE { get; set; }


        [ValueFilter]
        [BindProperty(Name = "OWNER_ID", SupportsGet = true)]
        public int? OWNER_ID { get; set; }
    }
}
