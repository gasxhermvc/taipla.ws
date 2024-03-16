using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Models.Responses.Frontend
{
    public class LoginResponse
    {
        public string token { get; set; }

        public DateTime expired { get; set; }
    }
}
