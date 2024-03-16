using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;

namespace Taipla.Webservice.Models.Responses.Backend.Media
{
    public class MediaResponse
    {
        public int MEDIA_ID { get; set; }
        public string FILENAME { get; set; }
        public UploadEnum PATH { get; set; }

        public string SYSTEM_NAME { get; set; }
        public string REF_ID { get; set; }
        public string URL { get; set; }

        public DateTime? CREATE_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
    }
}
