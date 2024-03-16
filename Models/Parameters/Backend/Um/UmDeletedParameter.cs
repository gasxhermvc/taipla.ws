using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Um
{
    public class UmDeletedParameter : BaseBackendParameter
    {
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "USER_ID is required.")]
        public long USER_ID { get; set; }
    }
}
