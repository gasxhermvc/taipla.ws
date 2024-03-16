using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Helpers.ExportExcel
{
    public static class ExcelExtensions
    {
        public static IServiceCollection AddExcelUtilities(this IServiceCollection services)
        {
            services.AddScoped<IExcelService, ExcelService>();

            return services;
        }

    }
}
