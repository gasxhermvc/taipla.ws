using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Taipla.Webservice.Extensions
{
    public static class DbExtension
    {
        public static string EncodeSpacialCharacters(this string spacialCharacters)
        {
            return HttpUtility.HtmlEncode(spacialCharacters);
        }

        public static string DecodeSpacialCharacters(this string rawSpacialCharacters)
        {
            return HttpUtility.HtmlDecode(rawSpacialCharacters);
        }
    }
}
