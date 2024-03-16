using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Responses.Backend.Country
{
    public class CountryResponse
    {
        public int COUNTRY_ID { get; set; }
        public int USER_ID { get; set; }
        public string NAME_TH { get; set; }
        public string NAME_EN { get; set; }
        public string DESCRIPTION { get; set; }
        public string THUMBNAIL { get; set; }
        public string AUTHOR { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }

        //=>Displayed
        public List<NzUploadFile> UPLOAD_FILES { get; set; }

    }
}
