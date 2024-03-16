using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Legend
{
    public class LegendDeletedParameter : BaseBackendParameter
    {
        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "LEGEND_ID is required.")]
        [BindProperty(Name = "LEGEND_ID", SupportsGet = true)]
        public int? LEGEND_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "CODE is required.")]
        [BindProperty(Name = "CODE", SupportsGet = true)]
        public string CODE { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "LEGEND_TYPE is required.")]
        [BindProperty(Name = "LEGEND_TYPE", SupportsGet = true)]
        public LegendEnum LEGEND_TYPE { get; set; }
    }
}
