using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Taipla.Webservice.Extensions
{
    public class PathExtension
    {
        public static string BasePath(IWebHostEnvironment web = null)
        {
            string path = string.Empty;
            if (web != null)
            {
                path = web.ContentRootPath;
            }
            else
            {
                path = AppDomain.CurrentDomain.BaseDirectory; ;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                path = path.Trim('/');
            }
            else
            {
                path = path.TrimEnd('/');
            }

            return path;

            
        }
    }
}
