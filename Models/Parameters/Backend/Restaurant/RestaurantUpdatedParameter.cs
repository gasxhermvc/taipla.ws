using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Attributes;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Parameters.Backend.Restaurant
{
    public class RestaurantUpdatedParameter : BaseBackendParameter
    {
        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "RES_ID is required.")]
        [BindProperty(Name = "RES_ID", SupportsGet = true)]
        public int? RES_ID { get; set; }

        [ValueFilter]
        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "COUNTRY_ID is required.")]
        [BindProperty(Name = "COUNTRY_ID", SupportsGet = true)]
        public int? COUNTRY_ID { get; set; }


        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PROVINCE is required.")]
        [BindProperty(Name = "PROVINCE", SupportsGet = true)]
        public string PROVINCE { get; set; }

        [BindRequired]
        [Required(AllowEmptyStrings = false, ErrorMessage = "NAME is required.")]
        [BindProperty(Name = "NAME", SupportsGet = true)]
        public string NAME { get; set; }

        [BindProperty(Name = "ADDRESS", SupportsGet = true)]
        public string ADDRESS { get; set; }

        [BindProperty(Name = "GOOGLE_MAP", SupportsGet = true)]
        public string GOOGLE_MAP { get; set; }

        [BindProperty(Name = "LATITUDE", SupportsGet = true)]
        public double? LATITUDE { get; set; }

        [BindProperty(Name = "LONGITUDE", SupportsGet = true)]
        public double? LONGITUDE { get; set; }

        [BindProperty(Name = "WEBSITE", SupportsGet = true)]
        public string WEBSITE { get; set; }

        [BindProperty(Name = "FACEBOOK", SupportsGet = true)]
        public string FACEBOOK { get; set; }

        [BindProperty(Name = "LINE", SupportsGet = true)]
        public string LINE { get; set; }

        [BindProperty(Name = "VIDEO", SupportsGet = true)]
        public string VIDEO { get; set; }

        [BindProperty(Name = "OPEN_TIME", SupportsGet = true)]
        public string OPEN_TIME { get; set; }

        [BindProperty(Name = "PHONE", SupportsGet = true)]
        public string PHONE { get; set; }

        [BindProperty(Name = "TAGS", SupportsGet = true)]
        public List<string> TAGS { get; set; }

        [BindProperty(Name = "CAR_PARK", SupportsGet = true)]
        public string CAR_PARK { get; set; }

        [BindProperty(Name = "UPLOAD", SupportsGet = true)]
        public IFormFile UPLOAD { get; set; }

        [ValueFilter]
        [BindProperty(Name = "OWNER_ID", SupportsGet = true)]
        public int? OWNER_ID { get; set; }

    }
}
