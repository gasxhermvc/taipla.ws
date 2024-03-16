using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Helpers.ExportExcel;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Responses.Backend.Export;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger _logger;
        private readonly IExcelService _excel;

        public ExportController(TAIPLA_DbContext dbContext,
            IAuthenticationService<UserInfo> authen,
            IHttpContextAccessor context,
            IWebHostEnvironment env,
            ILoggerFactory logger,
            IExcelService excel)
        {
            _dbContext = dbContext;
            _authen = authen;
            _context = context;
            _env = env;
            _logger = logger.CreateLogger("Service");
            _excel = excel;
        }

        [HttpGet("excel/food-center")]
        public IActionResult ExcelFoodCenter()
        {
            try
            {
                var countries = _dbContext.FoodCountry.ToList();
                var cultues = _dbContext.FoodCulture.ToList();
                var foods = _dbContext.FoodCenter.Where(w => w.UserId != 1).ToList();

                foods = (from food in foods
                         let image = _env.GetImageThumbnail(_context, food?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                         let newInstance = food.CopyTo<FoodCenter, FoodCenter>(new
                         {
                             Thumbnail = image.image
                         })
                         select newInstance).ToList();

                var _foods = (from food in foods
                              let country = countries.FirstOrDefault(f => f.CountryId == food.CountryId)
                              let culture = cultues.FirstOrDefault(f => f.CountryId == food.CountryId && f.CultureId == food.CultureId)
                              let newInstance = food.CopyTo<FoodCenter, FoodCenterExportResponse>(new
                              {
                                  CountryName = country.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                  CultureName = culture.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty
                              })
                              select newInstance).ToList();

                var config = _excel.Config("FoodCenter");

                var dt = _foods.ToDataTable(config.FIELD_NAME);

                var export = _excel.Export("FoodCenter", (config) =>
                {

                }
                , dt
                , (excelRange, column, value) =>
                {

                });

                if (export.success)
                {
                    return File(export.GetRawBytes(), export.MIME_TYPE, export.FILE_NAME);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Backend.ExportController.ExportExcelFoodCenter.Exception : {0}", e.ToString());
            }

            return BadRequest();
        }

        [HttpGet("excel/restaurant")]
        public IActionResult ExcelRestaurant()
        {
            try
            {

                var countries = _dbContext.FoodCountry.ToList();
                var cultues = _dbContext.FoodCulture.ToList();
                var restaurants = _dbContext.Restaurant.Where(w => w.UserId != 1).ToList();

                var userIds = restaurants.Select(s => s.UserId).ToList();
                var ownerIds = restaurants.Select(s => s.OwnerId.Value).ToList();
                var ids = userIds.Union(ownerIds).ToList();
                var users = _dbContext.User.Where(w => ids.Contains(w.UserId)).ToList();

                restaurants = (from res in restaurants
                               let image = _env.GetImageThumbnail(_context, res?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                               let newInstance = res.CopyTo<Restaurant, Restaurant>(new
                               {
                                   Thumbnail = image.image
                               })
                               select newInstance).ToList();


                var _res = (from res in restaurants
                            let country = countries.FirstOrDefault(f => f.CountryId == res.CountryId)
                            let user = users.FirstOrDefault(f => f.UserId == res.UserId)
                            let owner = users.FirstOrDefault(f => f.UserId == res.OwnerId)
                            let newInstance = res.CopyTo<Restaurant, RestaurantExportResponse>(new
                            {
                                UserCreate = string.Format("{0} {1}", user.FirstName.Trim() ?? string.Empty, user.LastName.Trim() ?? string.Empty).Trim(),
                                Owner = string.Format("{0} {1}", owner.FirstName.Trim() ?? string.Empty, owner.LastName.Trim() ?? string.Empty).Trim(),
                                IsCarPark = res.CarPark == 1 ? "Y" : "N",
                                CountryName = country.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty
                            })
                            select newInstance).ToList();


                var config = _excel.Config("Restaurant");

                var dt = _res.ToDataTable(config.FIELD_NAME);

                var export = _excel.Export("Restaurant", (config) =>
                {

                }
                , dt
                , (excelRange, column, value) =>
                {

                });

                if (export.success)
                {
                    return File(export.GetRawBytes(), export.MIME_TYPE, export.FILE_NAME);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Backend.ExportController.ExportExcelRestaurant.Exception : {0}", e.ToString());
            }

            return BadRequest();
        }

        [HttpGet("excel/restaurant-menu")]
        public IActionResult ExcelRestaurantMenu()
        {
            try
            {

                var countries = _dbContext.FoodCountry.ToList();
                var cultues = _dbContext.FoodCulture.ToList();
                var restaurants = _dbContext.Restaurant.Where(w => w.UserId != 1).ToList();

                var resIds = restaurants.Select(s => s.ResId).ToList();
                var restaurantMenus = _dbContext.RestaurantMenu.Where(w => resIds.Contains(w.ResId)).ToList();
                var codes = restaurantMenus.Select(s => s.Code).ToList();
                var legends = _dbContext.Legend.Where(w => codes.Contains(w.Code)).ToList();

                restaurants = (from res in restaurants
                               let image = _env.GetImageThumbnail(_context, res?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                               let newInstance = res.CopyTo<Restaurant, Restaurant>(new
                               {
                                   Thumbnail = image.image
                               })
                               select newInstance).ToList();

                restaurantMenus = (from resMenu in restaurantMenus
                                   let image = _env.GetImageThumbnail(_context, resMenu?.Thumbnail ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                                   let newInstance = resMenu.CopyTo<RestaurantMenu, RestaurantMenu>(new
                                   {
                                       Thumbnail = image.image
                                   })
                                   select newInstance).ToList();


                var _restaurantMenus = (from resMenu in restaurantMenus
                                        let res = restaurants.FirstOrDefault(f => f.ResId == resMenu.ResId)
                                        let country = countries.FirstOrDefault(f => f.CountryId == resMenu.CountryId)
                                        let culture = cultues.FirstOrDefault(f => f.CountryId == resMenu.CountryId && f.CultureId == resMenu.CultureId)
                                        let legend = legends.FirstOrDefault(f => f.Code == resMenu.Code)
                                        let newInstance = resMenu.CopyTo<RestaurantMenu, RestaurantMenuExportResponse>(new
                                        {
                                            ResName = res.Name?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                            CountryName = country.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                            CultureName = culture.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                            LegendDetail = legend?.Description?.Trim().DecodeSpacialCharacters() ?? string.Empty
                                        })
                                        select newInstance).ToList();


                var config = _excel.Config("RestaurantMenu");

                var dt = _restaurantMenus.ToDataTable(config.FIELD_NAME);

                var export = _excel.Export("RestaurantMenu", (config) =>
                {

                }
                , dt
                , (excelRange, column, value) =>
                {

                });

                if (export.success)
                {
                    return File(export.GetRawBytes(), export.MIME_TYPE, export.FILE_NAME);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Backend.ExportController.ExportExcelRestaurant.Exception : {0}", e.ToString());
            }

            return BadRequest();
        }
    }
}
