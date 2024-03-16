using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Frontend.Search
{
    public class SearchHistoryRemoveParameter : BaseFrontendParameter
    {

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "searchId is required.")]
        [BindProperty(Name = "searchId",SupportsGet = true)]
        public int SEARCH_ID { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "deviceId is required.")]
        [BindProperty(Name = "deviceId", SupportsGet = true)]
        public string DEVICE_ID { get; set; }
    }
}
