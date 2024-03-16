using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Models.Responses.Backend.Um
{
    public class UmUserResponse
    {
        public long USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
        public string ROLE { get; set; }
        public string AVATAR { get; set; }
        public string CLIENT_ID { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }

        //=>Displayed
        public string FULL_NAME { get; set; }
        public List<NzUploadFile> UPLOAD_FILES { get; set; }
    }
}
