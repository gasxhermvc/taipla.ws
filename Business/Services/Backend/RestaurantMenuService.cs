using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.RestaurantMenu;
using Taipla.Webservice.Models.Responses.Backend.RestaurantMenu;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class RestaurantMenuService : IRestaurantMenuService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        private BaseResponse response = new BaseResponse();

        public RestaurantMenuService(TAIPLA_DbContext dbContext,
            IAuthenticationService<UserInfo> authen,
            IHttpContextAccessor context,
            ILoggerFactory logger,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _authen = authen;
            _context = context;
            _logger = logger.CreateLogger("Service");
            _env = env;
            _configuration = configuration;
        }

        public async Task<BaseResponse> RestaurantMenus([FromQuery] RestaurantMenuRestaurantMenusParameter param)
        {
            try
            {
                if (param.COUNTRY_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสประเทศด้วย";
                    return this.response;
                }

                if (param.CULTURE_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสวัฒนธรรมอาหารด้วย";
                    return this.response;
                }

                if (param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสอาหารด้วย";
                    return this.response;
                }

                var resQuery = _dbContext.Restaurant.Where(w => w.ResId == param.RES_ID).AsQueryable();

                int USER_ID;

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        USER_ID = (param.OWNER_ID != null && param.OWNER_ID > 0 ? param.OWNER_ID.Value : -1);
                        break;
                    case RoleEnum.OWNER:
                        USER_ID = _authen.User.USER_ID;
                        break;
                    case RoleEnum.STAFF:
                        USER_ID = param.OWNER_ID.Value;
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }

                if (USER_ID != -1)
                {
                    resQuery = resQuery.Where(w => w.OwnerId == USER_ID);
                }

                var restaurant = resQuery.FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, เนื่องจากไม่พบข้อมูลร้านอาหาร";
                    return this.response;
                }

                var query = _dbContext.RestaurantMenu.Where(w => w.ResId == restaurant.ResId).AsQueryable();

                if (param.COUNTRY_ID != null)
                {
                    query = query.Where(w => w.CountryId == param.COUNTRY_ID);
                }

                if (param.CULTURE_ID != null)
                {
                    query = query.Where(w => w.CultureId == param.CULTURE_ID);
                }

                if (param.MENU_ID > 0)
                {
                    query = query.Where(w => w.MenuId == param.MENU_ID);
                }

                var restaurantMenus = query.ToList();

                List<RestaurantMenuResponse> responseData = new List<RestaurantMenuResponse>();

                if (restaurantMenus != null && restaurantMenus.Count < 1)
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูลเมนูอาหาร";
                    this.response.data = responseData;
                }
                else
                {
                    responseData = (from restaurantMenu in restaurantMenus
                                    let fileNameSplit = restaurantMenu.Thumbnail?.Split('/') ?? new string[] { }
                                    let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                    let ThumbnailUrl = !string.IsNullOrEmpty(restaurantMenu.Thumbnail) ? string.Format("{0}/{1}",
                                    _context.HttpContext.Request.GetUrl(_env).Trim(),
                                    restaurantMenu.Thumbnail) : string.Empty
                                    orderby restaurantMenu.CreatedDate descending
                                    select new RestaurantMenuResponse
                                    {
                                        MENU_ID = restaurantMenu.MenuId,
                                        RES_ID = restaurantMenu.ResId,
                                        COUNTRY_ID = restaurantMenu.CountryId,
                                        CULTURE_ID = restaurantMenu.CultureId,
                                        OWNER_ID = restaurant.OwnerId,
                                        NAME_TH = restaurantMenu.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        NAME_EN = restaurantMenu.NameEn,
                                        DESCRIPTION = restaurantMenu.Description?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        COOKING_FOOD = restaurantMenu.CookingFood,
                                        DIETETIC_FOOD = restaurantMenu.DieteticFood,
                                        CODE = restaurantMenu.Code,
                                        LEGEND_STATUS = restaurantMenu.LegendStatus,
                                        VIEWER = restaurantMenu.Viewer,
                                        THUMBNAIL = ThumbnailUrl,
                                        CREATED_DATE = restaurantMenu.CreatedDate,
                                        UPDATED_DATE = restaurantMenu.UpdatedDate
                                    }).ToList();

                    if (responseData.Count > 0)
                    {
                        var countryIds = responseData.Select(s => s.COUNTRY_ID).ToList();
                        var countries = _dbContext.CountryUtilities.GetCountries((FoodCountry country) => new
                        {
                            COUNTRY_ID = country.CountryId,
                            NAME_TH = country.NameTh
                        }, countryIds);

                        responseData.ForEach(f =>
                        {
                            var find = countries.FirstOrDefault(ff => ff.COUNTRY_ID == f.COUNTRY_ID);

                            if (find != null)
                            {
                                f.COUNTRY_NAME_TH = find.NAME_TH;
                            }
                        });

                        var cultureIds = responseData.Select(s => s.CULTURE_ID).ToList();
                        var cultures = _dbContext.CultureUtilities.GetCultures((FoodCulture culture) => new
                        {
                            CULTURE_ID = culture.CultureId,
                            CULTURE_NAME_TH = culture.NameTh
                        }, cultureIds);

                        responseData.ForEach(f =>
                        {
                            var find = cultures.FirstOrDefault(ff => ff.CULTURE_ID == f.CULTURE_ID);

                            if (find != null)
                            {
                                f.CULTURE_NAME_TH = find.CULTURE_NAME_TH;
                            }
                        });
                    }

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลอาหารสำเร็จ";
                    this.response.total = responseData.Count;
                    this.response.data = responseData;
                }

                this.response.success = true;
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantMenuService.RestaurantMenus.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> GetRestaurantMenu(RestaurantMenuRestaurantMenusParameter param)
        {
            try
            {
                if (param.COUNTRY_ID == null || param.COUNTRY_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสประเทศด้วย";
                    return this.response;
                }

                if (param.CULTURE_ID == null || param.CULTURE_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสวัฒนธรรมอาหารด้วย";
                    return this.response;
                }

                if (param.RES_ID == null || param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสอาหารด้วย";
                    return this.response;
                }

                var country = _dbContext.FoodCountry.FirstOrDefault(f => f.CountryId == param.COUNTRY_ID);

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";
                    return this.response;
                }

                var culture = _dbContext.FoodCulture.FirstOrDefault(f => f.CultureId == param.CULTURE_ID);

                if (culture == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";
                    return this.response;
                }

                int USER_ID;

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        USER_ID = (param.OWNER_ID != null && param.OWNER_ID > 0 ? param.OWNER_ID.Value : -1);
                        break;
                    case RoleEnum.OWNER:
                        USER_ID = _authen.User.USER_ID;
                        break;
                    case RoleEnum.STAFF:
                        USER_ID = param.OWNER_ID.Value;
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }

                var resQuery = _dbContext.Restaurant.Where(w => w.ResId == param.RES_ID).AsQueryable();
                if (USER_ID != -1)
                {
                    resQuery = resQuery.Where(w => w.OwnerId == USER_ID);
                }

                var restaurant = resQuery.FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, เนื่องจากไม่พบข้อมูลร้านอาหาร";
                    return this.response;
                }

                var query = _dbContext.RestaurantMenu.Where(w => w.MenuId == param.MENU_ID && w.ResId == restaurant.ResId).AsQueryable();
                var restaurantMenu = query.FirstOrDefault();

                if (restaurantMenu == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลเมนูอาหาร";
                    return this.response;
                }

                var fileNameSplit = restaurantMenu.Thumbnail?.Split('/') ?? new string[] { };
                string fileName = fileNameSplit?.LastOrDefault() ?? string.Empty;
                string ThumbnailUrl = !string.IsNullOrEmpty(restaurantMenu.Thumbnail) ? string.Format("{0}/{1}",
                            _context.HttpContext.Request.GetUrl(_env).Trim(),
                            restaurantMenu.Thumbnail) : string.Empty;

                var responData = new RestaurantMenuResponse
                {
                    MENU_ID = restaurantMenu.MenuId,
                    RES_ID = restaurantMenu.ResId,
                    COUNTRY_ID = restaurantMenu.CountryId,
                    CULTURE_ID = restaurantMenu.CultureId,
                    OWNER_ID = restaurant.OwnerId,
                    COUNTRY_NAME_TH = "",
                    CULTURE_NAME_TH = "",
                    NAME_TH = restaurantMenu.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    NAME_EN = restaurantMenu.NameEn,
                    DESCRIPTION = restaurantMenu.Description?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    COOKING_FOOD = restaurantMenu.CookingFood,
                    DIETETIC_FOOD = restaurantMenu.DieteticFood,
                    CODE = restaurantMenu.Code,
                    LEGEND_STATUS = restaurantMenu.LegendStatus,
                    VIEWER = restaurantMenu.Viewer,
                    PRICE = restaurantMenu.Price,
                    THUMBNAIL = restaurantMenu.Thumbnail,
                    CREATED_DATE = restaurantMenu.CreatedDate,
                    UPDATED_DATE = restaurantMenu.UpdatedDate,

                    UPLOAD_FILES = (!string.IsNullOrEmpty(ThumbnailUrl) ? new List<NzUploadFile> {
                        new NzUploadFile
                        {
                            name = fileName,
                            url = ThumbnailUrl,
                        } } : new List<NzUploadFile>())
                };

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = "ดึงข้อมูลอาหารสำเร็จ";
                this.response.data = responData;
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantMenuService.GetRestaurantMenu.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Created(RestaurantMenuCreatedParameter param)
        {
            try
            {
                if (param.RES_ID == null || param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
                    return this.response;
                }


                if (param.COUNTRY_ID == null || param.COUNTRY_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสประเทศด้วย";
                    return this.response;
                }

                if (param.CULTURE_ID == null || param.CULTURE_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสวัฒนธรรมอาหารด้วย";
                    return this.response;
                }

                var country = _dbContext.FoodCountry.FirstOrDefault(f => f.CountryId == param.COUNTRY_ID);

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";
                    return this.response;
                }

                var culture = _dbContext.FoodCulture.FirstOrDefault(f => f.CultureId == param.CULTURE_ID);

                if (culture == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลวัฒนธรรมอาหาร";
                    return this.response;
                }

                int USER_ID;
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        USER_ID = -1;
                        break;
                    case RoleEnum.OWNER:
                        USER_ID = _authen.User.USER_ID;
                        break;
                    case RoleEnum.STAFF:
                        USER_ID = param.OWNER_ID.Value;
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }

                var resQuery = _dbContext.Restaurant.Where(w => w.ResId == param.RES_ID).AsQueryable();
                if (USER_ID != -1)
                {
                    resQuery = resQuery.Where(w => w.OwnerId == USER_ID);
                }

                var restaurant = resQuery.FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, เนื่องจากไม่พบข้อมูลร้านอาหาร";
                    return this.response;
                }

                var restaurantMenu = _dbContext.RestaurantMenu.Where(w => w.NameTh == param.NAME_TH && w.ResId == restaurant.ResId)
                    .FirstOrDefault();

                if (restaurantMenu != null)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "สำเร็จ, มีรายการในระบบแล้ว";
                    return this.response;
                }

                var now = DateTime.Now;

                var CODE = $"{now.ToString("yyyyMMddHHmmssffff")}_" + Guid.NewGuid().ToString("N").Substring(0, int.Parse(_configuration["Application:MaxCodeLength"].ToString())); ;

                RestaurantMenu resMenu = new RestaurantMenu
                {
                    ResId = param.RES_ID.Value,
                    CountryId = param.COUNTRY_ID.Value,
                    CultureId = param.CULTURE_ID.Value,
                    NameTh = param.NAME_TH.EncodeSpacialCharacters(),
                    NameEn = param.NAME_EN,
                    Description = param.DESCRIPTION?.Trim() ?? string.Empty,
                    CookingFood = param.COOKING_FOOD?.Trim() ?? string.Empty,
                    DieteticFood = param.DIETETIC_FOOD?.Trim() ?? string.Empty,
                    Viewer = 0,
                    Code = CODE,
                    Price = param.PRICE,
                    //LegendStatus = !string.IsNullOrEmpty(param.LEGEND) ? 1 : 0,
                    CreatedDate = now,
                    UpdatedDate = now
                };

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.RestaurantMenu.Add(resMenu);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.RESTAURANT_MENU.GetString(),
                            fileName,
                            resMenu.MenuId.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.UPLOAD);

                        resMenu.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.RestaurantMenu.Update(resMenu);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.Created;
                    this.response.message = "บันทึกข้อมูลเมนูอาหารสำเร็จ";
                    this.response.data = new
                    {
                        OWNER_ID = restaurant.OwnerId,
                        MENU_ID = resMenu.MenuId,
                        RES_ID = resMenu.ResId,
                        CODE = resMenu.Code
                    };
                }
                else
                {
                    _logger.LogError("RestaurantMenuService.Created.Transaction.Exception: {0}", result.exception.ToString());
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantMenuService.Created.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Updated(RestaurantMenuUpdatedParameter param)
        {
            try
            {
                if (param.COUNTRY_ID == null || param.COUNTRY_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสประเทศด้วย";
                    return this.response;
                }

                if (param.CULTURE_ID == null || param.CULTURE_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสวัฒนธรรมอาหารด้วย";
                    return this.response;
                }

                if (param.RES_ID == null || param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสอาหารด้วย";
                    return this.response;
                }

                var country = _dbContext.FoodCountry.FirstOrDefault(f =>
                    f.CountryId == param.COUNTRY_ID);

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";

                    return this.response;
                }

                var culture = _dbContext.FoodCulture.FirstOrDefault(f =>
                 f.CultureId == param.CULTURE_ID);

                if (culture == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลวัฒธรรมอาหาร";

                    return this.response;
                }

                int USER_ID;
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        USER_ID = -1;
                        break;
                    case RoleEnum.OWNER:
                        USER_ID = _authen.User.USER_ID;
                        break;
                    case RoleEnum.STAFF:
                        USER_ID = param.OWNER_ID.Value;
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }

                var resQuery = _dbContext.Restaurant.Where(w => w.ResId == param.RES_ID).AsQueryable();
                if (USER_ID != -1)
                {
                    resQuery = resQuery.Where(w => w.OwnerId == USER_ID);
                }

                var restaurant = resQuery.FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, เนื่องจากไม่พบข้อมูลร้านอาหาร";
                    return this.response;
                }

                var Name = param.NAME_TH.EncodeSpacialCharacters();
                var query = _dbContext.RestaurantMenu.Where(w => w.MenuId == param.MENU_ID && w.ResId == restaurant.ResId).AsQueryable();

                var restaurantMenu = query.FirstOrDefault();

                if (restaurantMenu == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลเมนูอาหาร";

                    return this.response;
                }

                var now = DateTime.Now;
                restaurantMenu.CountryId = param.COUNTRY_ID.Value;
                restaurantMenu.CultureId = param.CULTURE_ID.Value;
                restaurantMenu.NameTh = param.NAME_TH.EncodeSpacialCharacters();
                restaurantMenu.NameEn = param.NAME_EN;
                restaurantMenu.Description = param.DESCRIPTION?.Trim() ?? string.Empty;
                restaurantMenu.CookingFood = param.COOKING_FOOD?.Trim() ?? string.Empty;
                restaurantMenu.DieteticFood = param.DIETETIC_FOOD?.Trim() ?? string.Empty;
                restaurantMenu.Price = param.PRICE;
                restaurantMenu.UpdatedDate = now;

                Media upload = null;
                Media oldUpload = _dbContext.UploadFileUtilities.GetMedia(
                     UploadEnum.RESTAURANT_MENU.GetString()
                     , restaurantMenu.MenuId.ToString()).CopyTo<Media, Media>();

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.RestaurantMenu.Update(restaurantMenu);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.RESTAURANT_MENU.GetString(),
                            restaurantMenu.MenuId.ToString());

                        if (upload == null)
                        {
                            upload = _dbContext.UploadFileUtilities.CreateMedia(
                                 UploadEnum.RESTAURANT_MENU.GetString(),
                                 fileName,
                                 restaurantMenu.MenuId.ToString(),
                                 now);

                            _dbContext.Media.Add(upload);
                            _dbContext.SaveChanges();
                        }
                        else
                        {
                            upload = _dbContext.UploadFileUtilities.UpdateMedia(upload, fileName);

                            _dbContext.Media.Update(upload);
                            _dbContext.SaveChanges();
                        }

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.UPLOAD);

                        restaurantMenu.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.RestaurantMenu.Update(restaurantMenu);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    if (oldUpload != null && upload != null && oldUpload.Filename != upload.Filename)
                    {
                        _dbContext.UploadFileUtilities.RemoveFile(oldUpload);
                    }

                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "แก้ไขข้อมูลเมนูอาหารสำเร็จ";
                    this.response.data = new
                    {
                        OWNER_ID = restaurant.OwnerId,
                        MENU_ID = restaurantMenu.MenuId,
                        RES_ID = restaurantMenu.ResId
                    };
                }
                else
                {
                    _logger.LogError("RestaurantMenuService.Updated.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantMenuService.Updated.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Deleted(RestaurantMenuDeletedParameter param)
        {
            try
            {
                if (param.COUNTRY_ID == null || param.COUNTRY_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสประเทศด้วย";
                    return this.response;
                }

                if (param.CULTURE_ID == null || param.CULTURE_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสวัฒนธรรมอาหารด้วย";
                    return this.response;
                }

                if (param.RES_ID == null || param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสอาหารด้วย";
                    return this.response;
                }

                var country = _dbContext.FoodCountry.FirstOrDefault(f =>
                    f.CountryId == param.COUNTRY_ID);

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";

                    return this.response;
                }

                var culture = _dbContext.FoodCulture.FirstOrDefault(f =>
                    f.CultureId == param.CULTURE_ID);

                if (culture == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลวัฒนธรรมอาหาร";

                    return this.response;
                }

                int USER_ID;
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        USER_ID = -1;
                        break;
                    case RoleEnum.OWNER:
                        USER_ID = _authen.User.USER_ID;
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }

                var resQuery = _dbContext.Restaurant.Where(w => w.ResId == param.RES_ID).AsQueryable();
                if (USER_ID != -1)
                {
                    resQuery = resQuery.Where(w => w.OwnerId == USER_ID);
                }

                var restaurant = resQuery.FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, เนื่องจากไม่พบข้อมูลร้านอาหาร";
                    return this.response;
                }

                var restaurantMenu = _dbContext.RestaurantMenu.Where(w => w.ResId == restaurant.ResId && w.MenuId == param.MENU_ID).FirstOrDefault();

                if (restaurantMenu == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลเมนูอาหาร";

                    return this.response;
                }

                var media = _dbContext.UploadFileUtilities.DeleteMedia(restaurantMenu.Thumbnail);

                var medias = _dbContext.UploadFileUtilities.GetMedias(UploadEnum.RESTAURANT_MENU.GetString(),
                    restaurantMenu.MenuId.ToString(), restaurantMenu.Thumbnail);

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.RestaurantMenu.Remove(restaurantMenu);
                    if (media != null)
                    {
                        _dbContext.Media.Remove(media);

                        //=>ต้องการลบไฟล์ด้วย
                        //_dbContext.UploadFileUtilities.RemoveFile(media);
                        // _dbContext.UploadFileUtilities.RemoveFolder(restaurantMenu.Thumbnail);
                    }

                    if (medias != null && medias.Count > 0)
                    {
                        _dbContext.Media.RemoveRange(medias);
                    }

                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    _dbContext.UploadFileUtilities.RemoveFolder(restaurantMenu.Thumbnail);
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ลบข้อมูลเมนูอาหารสำเร็จ";
                    this.response.data = new
                    {
                        OWNER_ID = restaurant.OwnerId,
                        MENU_ID = restaurantMenu.MenuId,
                        RES_ID = restaurantMenu.ResId
                    };
                }
                else
                {
                    _logger.LogError("RestaurantMenuService.Deleted.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantMenuService.Deleted.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        //=>new 

        public async Task<BaseResponse> Medias(RestaurantMenuRestaurantMenusParameter param)
        {
            List<NzUploadFile> UPLOAD_FILES = new List<NzUploadFile>();

            try
            {
                if (param.COUNTRY_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสประเทศด้วย";
                    return this.response;
                }

                if (param.CULTURE_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสวัฒนธรรมอาหารด้วย";
                    return this.response;
                }

                if (param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสอาหารด้วย";
                    return this.response;
                }

                var restaurantMenu = _dbContext.RestaurantMenu.FirstOrDefault(f => f.ResId == param.RES_ID &&
                    f.CountryId == param.COUNTRY_ID &&
                    f.CultureId == param.CULTURE_ID);

                if (restaurantMenu == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลเมนูอาหาร";

                    return this.response;
                }

                var medias = _dbContext.UploadFileUtilities.GetMedias(UploadEnum.RESTAURANT_MENU.GetString()
                    , restaurantMenu.MenuId.ToString()
                    , restaurantMenu.Thumbnail);


                UPLOAD_FILES = (from m in medias
                                let fileNameSplit = m.Path?.Split('/') ?? new string[] { }
                                let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                let ThumbnailUrl = !string.IsNullOrEmpty(m.Filename) ? string.Format("{0}/{1}",
                                    _context.HttpContext.Request.GetUrl(_env).Trim(),
                                    m.Path) : string.Empty
                                select new NzUploadFile
                                {
                                    name = m.Filename,
                                    url = ThumbnailUrl
                                }).ToList();


                if (UPLOAD_FILES != null && UPLOAD_FILES.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ, แต่ไม่พบข้อมูล";
                }
                this.response.total = UPLOAD_FILES.Count;
                this.response.data = UPLOAD_FILES;
                this.response.success = true;

            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantMenuService.Medias.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }
    }
}
