using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Helpers.DbUtilities
{
    public static class DbUtilityExtensions
    {
        public static IServiceCollection AddDbUtilities(this IServiceCollection services)
        {
            services.AddScoped<IDbUtilityService, DbUtilityService>();

            return services;
        }

        public static IServiceCollection AddDbUtilities(this IServiceCollection services, IConfiguration confinguration)
        {
            services.AddScoped<IDbUtilityService, DbUtilityService>();

            return services;
        }

        //public static IApplicationBuilder UseAddDbUtilities(this IApplicationBuilder app)
        //{
        //    return app;
        //}
    }
}
