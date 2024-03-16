using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Responses.Backend.Promotion
{
    public class PromotionResponse
    {
        public int PROMOTION_ID { get; set; }

        public int RES_ID { get; set; }

        public string NAME { get; set; }

        public string DESCRIPTION { get; set; }

        public string THUMBNAIL { get; set; }

        public string PROMOTION_TYPE { get; set; }

        public string PROMOTION_TYPE_DESC { get; set; }

        public int VIEWER { get; set; }

        public string START_DATE { get; set; }

        public string END_DATE { get; set; }

        public DateTime? CREATE_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }

        //=>Displayed
        public List<NzUploadFile> UPLOAD_FILES { get; set; }
    }
}
