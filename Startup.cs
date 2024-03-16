using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Entities;
using InterfacesBackend = Taipla.Webservice.Business.Interfaces.Backend;
using InterfacesFrontend = Taipla.Webservice.Business.Interfaces.Frontend;
using ServiceBackend = Taipla.Webservice.Business.Services.Backend;
using ServiceFrontend = Taipla.Webservice.Business.Services.Frontend;
using Taipla.Webservice.Helpers.DbUtilities;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Filters.Middlewares;
using Taipla.Webservice.Helpers.ExportExcel;

namespace Taipla.Webservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builderConfig = new ConfigurationBuilder()
              .SetBasePath(env.ContentRootPath)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builderConfig.Build();
            configuration = Configuration;

            Console.WriteLine("Evnironment Name : {0}", env.EnvironmentName);
            Console.WriteLine("SQL Connection string : {0}", Configuration.GetConnectionString("DefaultConnection"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                //=>Allowd sizeof 30MB
                options.ValueCountLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartBoundaryLengthLimit = int.MaxValue;
            });

            services.AddHttpContextAccessor();
            services.AddDbContext<TAIPLA_DbContext>(options =>
               options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddJWTAuthentication(Configuration, options =>
            {
                options.AddScoped(typeof(IAuthenticationService<UserInfo>), typeof(UserContext<UserInfo>));
                options.AddScoped(typeof(IAuthenticationService<ClientInfo>), typeof(ClientContext<ClientInfo>));

            }); //=>CUSTOM

            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                options.UseApiBehavior = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddSpaStaticFiles(options =>
            {
                options.RootPath = "wwwroot/dist";
            });

            //=>inject
            services.AddSingleton<MessageFactory>();
            services.AddSession();
            services.AddDetection();
            services.AddExcelUtilities();
            services.AddDbUtilities(); //=>CUSTOM

            //=>frontends
            services.AddScoped<InterfacesFrontend.IAuthService, ServiceFrontend.AuthService>();
            services.AddScoped<InterfacesFrontend.IAccountService, ServiceFrontend.AccountService>();
            services.AddScoped<InterfacesFrontend.IFirstPageService, ServiceFrontend.FirstPageService>();
            services.AddScoped<InterfacesFrontend.ISearchService, ServiceFrontend.SearchService>();
            services.AddScoped<InterfacesFrontend.IFoodCenterService, ServiceFrontend.FoodCenterService>();
            services.AddScoped<InterfacesFrontend.IRestaurantService, ServiceFrontend.RestaurantService>();
            services.AddScoped<InterfacesFrontend.ILogService, ServiceFrontend.LogService>();

            //=>backend
            services.AddScoped<InterfacesBackend.IAuthService, ServiceBackend.AuthService>();
            services.AddScoped<InterfacesBackend.IUmService, ServiceBackend.UmService>();
            services.AddScoped<InterfacesBackend.ILUTService, ServiceBackend.LUTService>();
            services.AddScoped<InterfacesBackend.ICountryService, ServiceBackend.CountryService>();
            services.AddScoped<InterfacesBackend.ICultureService, ServiceBackend.CultureService>();
            services.AddScoped<InterfacesBackend.IFoodService, ServiceBackend.FoodService>();
            services.AddScoped<InterfacesBackend.IFoodIngredientService, ServiceBackend.FoodIngredientService>();
            services.AddScoped<InterfacesBackend.IRestaurantService, ServiceBackend.RestaurantService>();
            services.AddScoped<InterfacesBackend.IRestaurantMenuService, ServiceBackend.RestaurantMenuService>();
            services.AddScoped<InterfacesBackend.ILegendService, ServiceBackend.LegendService>();
            services.AddScoped<InterfacesBackend.IPromotionService, ServiceBackend.PromotionService>();
            services.AddScoped<InterfacesBackend.IMediaService, ServiceBackend.MediaService>();
            services.AddCors(options =>
               options.AddPolicy("TAIPLA", builder =>
               {
                   builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
               }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestResponseLoggingMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(options =>
              options.WithOrigins("*")
              .AllowAnyHeader()
              .AllowAnyMethod());
            app.UseCors("TAIPLA");

            app.UseExceptionHandlingMiddleware();

            app.UseSession();

            app.UseDetection();

            app.UseJWTAuthentication();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpaStaticFiles();
            app.UseSpa(options =>
            {
                if (env.IsDevelopment())
                {
                    options.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}
