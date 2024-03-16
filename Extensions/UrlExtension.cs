using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Extensions
{
    public static class UrlExtension
    {
        public static string GetHostName(this HttpRequest request)
        {
            return string.Format("{0}", request.Host);
        }

        public static string GetUrl(this HttpRequest request, IWebHostEnvironment env)
        {
            var schema = request.Scheme;
            if (env.IsProduction())
            {
                schema = schema == "http" ? "https" : schema;
            }

            return env.IsProduction() ? string.Format($"{schema}://{request.Host}/api") :
                 string.Format($"{schema}://{request.Host}");
        }
    }
}
