using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Responses.Backend.Restaurant
{
    public class RestaurantResponse
    {
        public int RES_ID { get; set; }
        public int COUNTRY_ID { get; set; }
        public string COUNTRY_NAME_TH { get; set; }

        public string PROVINCE { get; set; }
        public string NAME { get; set; }

        public string ADDRESS { get; set; }
        public string GOOGLE_MAP { get; set; }
        public double? LATITUDE { get; set; }
        public double? LONGITUDE { get; set; }
        public string WEBSITE { get; set; }
        public string FACEBOOK { get; set; }
        public string LINE { get; set; }
        public string VIDEO { get; set; }
        public string OPEN_TIME { get; set; }
        public string PHONE { get; set; }
        public List<string> TAGS { get; set; }
        public string CAR_PARK { get; set; }
        public int VIEWER { get; set; }
        public string THUMBNAIL { get; set; }
        public int USER_ID { get; set; }
        public int? OWNER_ID { get; set; }
        public string AUTHOR { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }

        //=>Displayed
        public string OWNER { get; set; }
        public List<NzUploadFile> UPLOAD_FILES { get; set; }
    }
}
