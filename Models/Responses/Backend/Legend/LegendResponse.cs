using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Responses.Backend.Legend
{
    public class LegendResponse
    {
        public int LEGEND_ID { get; set; }

        public string DESCRIPTION { get; set; }

        public string CODE { get; set; }

        public string THUMBNAIL { get; set; }

        public string LEGEND_TYPE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }

        //=>Displayed
        public List<NzUploadFile> UPLOAD_FILES { get; set; }
    }
}
