using Microsoft.AspNetCore.Http;
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
    public class PromotionUpdatedParameter : BaseBackendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PROMOTION_ID is required.")]
        [BindProperty(Name = "PROMOTION_ID", SupportsGet = true)]
        [ValueFilter]
        public int? PROMOTION_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "RES_ID is required.")]
        [BindProperty(Name = "RES_ID", SupportsGet = true)]
        [ValueFilter]
        public int? RES_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "NAME is required.")]
        [BindProperty(Name = "NAME", SupportsGet = true)]
        public string NAME { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "DESCRIPTION is required.")]
        [BindProperty(Name = "DESCRIPTION", SupportsGet = true)]
        public string DESCRIPTION { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PROMOTION_TYPE is required.")]
        [BindProperty(Name = "PROMOTION_TYPE", SupportsGet = true)]
        public PromotionEnum PROMOTION_TYPE { get; set; }

        [BindProperty(Name = "START_DATE", SupportsGet = true)]
        public DateTime? START_DATE { get; set; }

        [BindProperty(Name = "END_DATE", SupportsGet = true)]
        public DateTime? END_DATE { get; set; }

        public IFormFile UPLOAD { get; set; }

        [ValueFilter]
        [BindProperty(Name = "OWNER_ID", SupportsGet = true)]
        public int? OWNER_ID { get; set; }
    }
}
