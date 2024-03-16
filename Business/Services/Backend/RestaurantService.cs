using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using Taipla.Webservice.Models.Parameters.Backend.Restaurant;
using Taipla.Webservice.Models.Responses.Backend.Restaurant;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class RestaurantService : IRestaurantService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        private BaseResponse response = new BaseResponse();

        public RestaurantService(TAIPLA_DbContext dbContext,
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

        public async Task<BaseResponse> Restaurants(RestaurantRestaurantsParameter param)
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

                if (param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
                    return this.response;
                }

                if (param.AUTHOR < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสผู้สร้างรายการร้านอาหารด้วย";
                    return this.response;
                }

                var query = _dbContext.Restaurant.AsQueryable();

                if (param.COUNTRY_ID != null)
                {
                    query = query.Where(w => w.CountryId == param.COUNTRY_ID);
                }

                if (param.RES_ID != null)
                {
                    query = query.Where(w => w.ResId == param.RES_ID);
                }


                if (param.AUTHOR != null)
                {
                    query = query.Where(w => w.UserId == param.AUTHOR);
                }

                if (!string.IsNullOrEmpty(param.NAME))
                {
                    var name = param.NAME.EncodeSpacialCharacters();

                    query = query.Where(w => w.Name.StartsWith(name) || w.Name.Contains(name) || w.Name.EndsWith(name));
                }

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                        break;
                    case RoleEnum.POST_RESTAURANT:
                        query = query.Where(w => w.UserId == _authen.User.USER_ID);
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }

                var restaurants = query.ToList();

                List<RestaurantResponse> responseData = new List<RestaurantResponse>();

                if (restaurants != null && restaurants.Count < 1)
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูลร้านอาหาร";
                    this.response.data = responseData;
                }
                else
                {
                    responseData = (from restaurant in restaurants
                                    let fileNameSplit = restaurant.Thumbnail?.Split('/') ?? new string[] { }
                                    let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                    let ThumbnailUrl = !string.IsNullOrEmpty(restaurant.Thumbnail) ? string.Format("{0}/{1}",
                                    _context.HttpContext.Request.GetUrl(_env).Trim(),
                                    restaurant.Thumbnail) : string.Empty
                                    orderby restaurant.CreatedDate descending
                                    select new RestaurantResponse
                                    {
                                        RES_ID = restaurant.ResId,
                                        COUNTRY_ID = restaurant.CountryId,
                                        NAME = restaurant.Name?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        ADDRESS = restaurant.Address?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        GOOGLE_MAP = restaurant.Map?.Trim() ?? string.Empty,
                                        LATITUDE = restaurant.Latitude,
                                        LONGITUDE = restaurant.Longitude,
                                        WEBSITE = restaurant.Website?.Trim() ?? string.Empty,
                                        FACEBOOK = restaurant.Facebook?.Trim() ?? string.Empty,
                                        LINE = restaurant.Line?.Trim() ?? string.Empty,
                                        OPEN_TIME = restaurant.OpenTime?.Trim() ?? string.Empty,
                                        PHONE = restaurant.Phone?.Trim() ?? string.Empty,
                                        TAGS = restaurant.Tags?.Trim().Split(',').Where(w => !string.IsNullOrEmpty(w)).Select(s => s.DecodeSpacialCharacters()).ToList() ?? new List<string>(),
                                        CAR_PARK = restaurant.CarPark == 1 ? "1" : "0",
                                        VIEWER = restaurant.Viewer,
                                        THUMBNAIL = ThumbnailUrl,
                                        AUTHOR = "",
                                        USER_ID = restaurant.UserId,
                                        OWNER_ID = restaurant.OwnerId,
                                        CREATE_DATE = restaurant.CreatedDate,
                                        UPDATE_DATE = restaurant.UpdatedDate
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

                        var userIds = responseData.Select(s => s.USER_ID).ToList();
                        var users = _dbContext.UserUtilities.GetUsers((User user) => new
                        {
                            USER_ID = user.UserId,
                            USERNAME = user.Username
                        }, userIds);

                        responseData.ForEach(f =>
                        {
                            var find = users.FirstOrDefault(ff => ff.USER_ID == f.USER_ID);

                            if (find != null)
                            {
                                f.AUTHOR = find.USERNAME;
                            }
                        });

                        var ownerIds = responseData.Where(w => w.OWNER_ID != null)
                            .Select(s => s.OWNER_ID.Value).ToList();

                        if (ownerIds != null && ownerIds.Count > 0)
                        {
                            var owners = _dbContext.UserUtilities.GetUsers((User user) => new
                            {
                                USER_ID = user.UserId,
                                USERNAME = user.Username,
                                FULL_NAME = string.Format("{0} {1}",
                                    (!string.IsNullOrEmpty(user.FirstName) ? user.FirstName.Trim() : string.Empty)
                                  , (!string.IsNullOrEmpty(user.LastName) ? user.LastName.Trim() : string.Empty)).Trim()
                            }, ownerIds);
                            responseData.ForEach(f =>
                            {
                                var find = owners.FirstOrDefault(ff => ff.USER_ID == f.OWNER_ID);

                                if (find != null)
                                {
                                    f.OWNER = find.FULL_NAME;
                                }
                            });
                        }

                    }

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลร้านอาหารสำเร็จ";
                    this.response.total = responseData.Count;
                    this.response.data = responseData;
                }

                this.response.success = true;
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantService.Restaurants.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> GetRestaurant(RestaurantRestaurantsParameter param)
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

                if (param.RES_ID == null || param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
                    return this.response;
                }

                ////if (param.OWNER_ID == null || param.OWNER_ID < 1)
                ////{
                ////    this.response.success = false;
                ////    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                ////    this.response.message = "ไม่สำเร็จ, ต้องการรหัสเจ้าของร้านอาหารด้วย";
                ////    return this.response;
                ////}

                var country = _dbContext.FoodCountry.FirstOrDefault(f => f.CountryId == param.COUNTRY_ID);

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";
                    return this.response;
                }

                var query = _dbContext.Restaurant.Where(w => w.ResId == param.RES_ID).AsQueryable();

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                        if (param.OWNER_ID != -1)
                        {
                            query = query.Where(w => w.OwnerId == param.OWNER_ID);
                        }
                        break;
                    case RoleEnum.POST_RESTAURANT:
                        query = query.Where(w => w.UserId == _authen.User.USER_ID);
                        if (param.OWNER_ID != -1)
                        {
                            query = query.Where(w => w.OwnerId == param.OWNER_ID);
                        }
                        break;
                    case RoleEnum.OWNER:
                        query = query.Where(w => w.OwnerId == _authen.User.USER_ID);
                        break;
                    case RoleEnum.STAFF:
                        query = query.Where(w => w.OwnerId == param.OWNER_ID);
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }
                var restaurant = query.FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลร้านอาหาร";
                    return this.response;
                }

                var fileNameSplit = restaurant.Thumbnail?.Split('/') ?? new string[] { };
                string fileName = fileNameSplit?.LastOrDefault() ?? string.Empty;
                string ThumbnailUrl = !string.IsNullOrEmpty(restaurant.Thumbnail) ? string.Format("{0}/{1}",
                            _context.HttpContext.Request.GetUrl(_env).Trim(),
                            restaurant.Thumbnail) : string.Empty;

                var responData = new RestaurantResponse
                {
                    RES_ID = restaurant.ResId,
                    COUNTRY_ID = restaurant.CountryId,
                    COUNTRY_NAME_TH = country.NameTh.Trim(),
                    NAME = restaurant.Name.Trim().DecodeSpacialCharacters(),
                    ADDRESS = restaurant.Address?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    PROVINCE = restaurant.Province?.Trim() ?? string.Empty,
                    GOOGLE_MAP = restaurant.Map?.Trim() ?? string.Empty,
                    LATITUDE = restaurant.Latitude,
                    LONGITUDE = restaurant.Longitude,
                    WEBSITE = restaurant.Website?.Trim() ?? string.Empty,
                    FACEBOOK = restaurant.Facebook?.Trim() ?? string.Empty,
                    LINE = restaurant.Line?.Trim() ?? string.Empty,
                    VIDEO = restaurant.Video?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    OPEN_TIME = restaurant.OpenTime?.Trim() ?? string.Empty,
                    PHONE = restaurant.Phone?.Trim() ?? string.Empty,
                    TAGS = restaurant.Tags?.Trim().Split(',').Where(w => !string.IsNullOrEmpty(w)).Select(s => s.DecodeSpacialCharacters()).ToList() ?? new List<string>(),
                    CAR_PARK = restaurant.CarPark == 1 ? "1" : "0",
                    VIEWER = restaurant.Viewer,
                    THUMBNAIL = ThumbnailUrl,
                    AUTHOR = "",
                    USER_ID = restaurant.UserId,
                    OWNER_ID = restaurant.OwnerId,
                    CREATE_DATE = restaurant.CreatedDate,
                    UPDATE_DATE = restaurant.UpdatedDate,

                    UPLOAD_FILES = (!string.IsNullOrEmpty(ThumbnailUrl) ? new List<NzUploadFile> {
                        new NzUploadFile
                        {
                            name = fileName,
                            url = ThumbnailUrl,
                        } } : new List<NzUploadFile>())
                };

                var owners = _dbContext.UserUtilities.GetUsers((User user) => new
                {
                    USER_ID = user.UserId,
                    USERNAME = user.Username,
                    FULL_NAME = string.Format("{0} {1}",
                        (!string.IsNullOrEmpty(user.FirstName) ? user.FirstName.Trim() : string.Empty)
                      , (!string.IsNullOrEmpty(user.LastName) ? user.LastName.Trim() : string.Empty)).Trim()
                }, new List<int>() { responData.OWNER_ID.Value });


                var find = owners.FirstOrDefault(ff => ff.USER_ID == responData.OWNER_ID.Value);

                if (find != null)
                {
                    responData.AUTHOR = find.FULL_NAME;
                }

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = "ดึงข้อมูลร้านอาหารสำเร็จ";
                this.response.data = responData;
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantService.GetRestaurant.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Created(RestaurantCreatedParameter param)
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

                var country = _dbContext.FoodCountry.FirstOrDefault(f => f.CountryId == param.COUNTRY_ID);

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";
                    return this.response;
                }

                if (string.IsNullOrEmpty(param.PROVINCE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่พบข้อมูลจังหวัด";
                    return this.response;
                }

                var restaurantName = param.NAME.EncodeSpacialCharacters();

                var restaurant = _dbContext.Restaurant.Where(w => w.Name == restaurantName && w.OwnerId == param.OWNER_ID)
                    .FirstOrDefault();

                if (restaurant != null)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "สำเร็จ, มีรายการในระบบแล้ว";
                    return this.response;
                }

                //=>ปกติต้องมีแต่เอาออกเพื่อให้กรอกก่อน
                ////var IsOwner = _dbContext.User.Where(w => w.UserId == param.OWNER_ID && w.Role == RoleEnum.OWNER.GetString()).FirstOrDefault();
                ////if (IsOwner == null)
                ////{
                ////    this.response.success = false;
                ////    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                ////    this.response.message = "ไม่สำเร็จ, เนื่องจาก OWNER_ID ไม่ถูกต้อง";
                ////    return this.response;
                ////}


                ////var IsOwnerDuplicate = _dbContext.Restaurant.Where(w => w.OwnerId == param.OWNER_ID).FirstOrDefault();

                ////if (IsOwnerDuplicate != null)
                ////{
                ////    this.response.success = false;
                ////    this.response.statusCode = (int)HttpStatusCode.Conflict;
                ////    this.response.message = "ไม่สำเร็จ, เนื่องจากมีชื่อเจ้าของร้านอาหารนี้ในระบบแล้ว";
                ////    return this.response;
                ////}

                var now = DateTime.Now;

                Restaurant res = new Restaurant
                {
                    CountryId = param.COUNTRY_ID.Value,
                    Province = param.PROVINCE.Trim(),
                    Name = param.NAME.EncodeSpacialCharacters(),
                    Address = (param.ADDRESS?.Trim() ?? string.Empty).EncodeSpacialCharacters(),
                    Map = param.GOOGLE_MAP?.Trim() ?? string.Empty,
                    Latitude = param.LATITUDE,
                    Longitude = param.LONGITUDE,
                    Website = param.WEBSITE?.Trim() ?? string.Empty,
                    Facebook = param.FACEBOOK?.Trim() ?? string.Empty,
                    Line = param.LINE?.Trim() ?? string.Empty,
                    OpenTime = param.OPEN_TIME?.Trim() ?? string.Empty,
                    Phone = param.PHONE?.Trim() ?? string.Empty,
                    Tags = string.Join(",", param.TAGS.Where(w => !string.IsNullOrEmpty(w)).Select(s => s.EncodeSpacialCharacters())),
                    Video = param.VIDEO?.Trim().EncodeSpacialCharacters() ?? string.Empty,
                    CarPark = param.CAR_PARK == "1" ? 1 : 0,
                    Viewer = 0,
                    UserId = _authen.User.USER_ID,
                    OwnerId = param.OWNER_ID != null ? param.OWNER_ID.Value : (int?)null,
                    CreatedDate = now,
                    UpdatedDate = now,
                };

                ////var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                ////switch (role)
                ////{
                ////    case RoleEnum.SUPER_ADMIN: //=>ผู้ดูแล
                ////    case RoleEnum.ADMIN:
                ////    case RoleEnum.POST: //=>ผู้สร้างข้อมูลทั่วไป
                ////        restaurant.OwnerId = _authen.User.USER_ID;
                ////        break;
                ////    default:
                ////        this.response.success = true;
                ////        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                ////        this.response.message = "ไม่สำเร็จ, ไม่มีสิทธิ์ทำรายการ";
                ////        return this.response;
                ////}

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Restaurant.Add(res);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.RESTAURANT.GetString(),
                            fileName,
                            res.ResId.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.UPLOAD);

                        res.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.Restaurant.Update(res);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.Created;
                    this.response.message = "บันทึกข้อมูลร้านอาหารสำเร็จ";
                    this.response.data = new
                    {
                        RES_ID = res.ResId
                    };
                }
                else
                {
                    _logger.LogError("RestaurantService.Created.Transaction.Exception: {0}", result.exception.ToString());
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantService.Created.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Updated(RestaurantUpdatedParameter param)
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

                if (param.RES_ID == null || param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
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

                if (string.IsNullOrEmpty(param.PROVINCE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่พบข้อมูลจังหวัด";

                    return this.response;
                }

                var restaurantName = param.NAME.EncodeSpacialCharacters();

                var query = _dbContext.Restaurant.Where(w => w.ResId == param.RES_ID).AsQueryable();
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                        break;
                    case RoleEnum.POST_RESTAURANT:
                        query = query.Where(w => w.UserId == _authen.User.USER_ID);
                        break;
                    case RoleEnum.OWNER:
                        query = query.Where(w => w.OwnerId == _authen.User.USER_ID);
                        break;
                    case RoleEnum.STAFF:
                        query = query.Where(w => w.OwnerId == param.OWNER_ID);
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }

                var restaurant = query.FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลร้านอาหาร";

                    return this.response;
                }

                var now = DateTime.Now;
                restaurant.CountryId = param.COUNTRY_ID.Value;
                restaurant.Province = param.PROVINCE.Trim();
                restaurant.Name = param.NAME;
                restaurant.Address = param.ADDRESS?.Trim() ?? string.Empty;
                restaurant.Map = param.GOOGLE_MAP;
                restaurant.Latitude = param.LATITUDE;
                restaurant.Longitude = restaurant.Longitude;
                restaurant.Website = param.WEBSITE;
                restaurant.Facebook = param.FACEBOOK;
                restaurant.Line = param.LINE;
                restaurant.Video = param.VIDEO?.Trim().EncodeSpacialCharacters() ?? string.Empty;
                restaurant.OpenTime = param.OPEN_TIME;
                restaurant.Phone = param.PHONE;
                restaurant.Tags = string.Join(",", param.TAGS.Where(w => !string.IsNullOrEmpty(w)).Select(s => s.EncodeSpacialCharacters()));
                restaurant.CarPark = param.CAR_PARK == "1" ? 1 : 0;
                restaurant.Viewer = 0;
                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                        break;
                    case RoleEnum.POST_RESTAURANT:
                        restaurant.UserId = _authen.User.USER_ID; ;
                        break;
                    case RoleEnum.OWNER:
                    case RoleEnum.STAFF:
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }
                restaurant.CreatedDate = now;
                restaurant.UpdatedDate = now;

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Restaurant.Update(restaurant);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.FOOD_CENTER.GetString(),
                            restaurant.ResId.ToString());


                        if (upload == null)
                        {
                            upload = _dbContext.UploadFileUtilities.CreateMedia(
                                 UploadEnum.FOOD_CENTER.GetString(),
                                 fileName,
                                 restaurant.ResId.ToString(),
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

                        restaurant.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.Restaurant.Update(restaurant);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "แก้ไขข้อมูลร้านอาหารสำเร็จ";
                    this.response.data = new
                    {
                        RES_ID = restaurant.ResId
                    };
                }
                else
                {
                    _logger.LogError("RestaurantService.Updated.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantService.Updated.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Deleted(RestaurantDeletedParameter param)
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

                if (param.RES_ID == null || param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
                    return this.response;
                }

                ////if (param.OWNER_ID == null || param.OWNER_ID < 1)
                ////{
                ////    this.response.success = false;
                ////    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                ////    this.response.message = "ไม่สำเร็จ, ต้องการรหัสเจ้าของร้านด้วย";
                ////    return this.response;
                ////}

                var country = _dbContext.FoodCountry.FirstOrDefault(f =>
                    f.CountryId == param.COUNTRY_ID);

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";

                    return this.response;
                }

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์ลบข้อมูล";
                        this.response.data = null;
                        return this.response;
                }

                var restaurant = _dbContext.Restaurant.Where(w => w.ResId == param.RES_ID && w.OwnerId == param.OWNER_ID).FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, เนื่องจากไม่พบข้อมูลร้านอาหาร";

                    return this.response;
                }

                var media = _dbContext.UploadFileUtilities.DeleteMedia(restaurant.Thumbnail);

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Restaurant.Remove(restaurant);
                    if (media != null)
                    {
                        _dbContext.Media.Remove(media);
                        //=>ต้องการลบไฟล์ด้วย
                        //_dbContext.UploadFileUtilities.RemoveFile(media);
                        // _dbContext.UploadFileUtilities.RemoveFolder(foodCenter.Thumbnail);
                    }
                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    _dbContext.UploadFileUtilities.RemoveFolder(restaurant.Thumbnail);
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ลบข้อมูลร้านอาหารสำเร็จ";
                    this.response.data = new
                    {
                        RES_ID = restaurant.ResId
                    };
                }
                else
                {
                    _logger.LogError("RestaurantService.Deleted.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("RestaurantService.Deleted.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Medias(RestaurantRestaurantsParameter param)
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

                if (param.OWNER_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสเจ้าของร้านอาหารด้วย";
                    return this.response;
                }

                if (param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
                    return this.response;
                }

                var query = _dbContext.Restaurant.Where(f => f.ResId == param.RES_ID && f.CountryId == param.COUNTRY_ID).AsQueryable();
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                        break;
                    case RoleEnum.POST_RESTAURANT:
                        query = query.Where(w => w.UserId == _authen.User.USER_ID);
                        break;
                    case RoleEnum.OWNER:
                        query = query.Where(w => w.OwnerId == _authen.User.USER_ID);
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }

                var restaurant = query.FirstOrDefault();

                if (restaurant == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลร้านอาหาร";

                    return this.response;
                }

                var restaurantMenu = _dbContext.RestaurantMenu.Where(w => w.ResId == restaurant.ResId).Select(s => s.MenuId.ToString()).ToList();

                var restaurantMedia = _dbContext.UploadFileUtilities.GetMedias(UploadEnum.RESTAURANT.GetString()
                    , restaurant.ResId.ToString()
                    , restaurant.Thumbnail);


                var menuRestaurantMedia = _dbContext.UploadFileUtilities.GetMedias(UploadEnum.RESTAURANT_MENU.GetString()
                 , restaurantMenu
                 , restaurant.Thumbnail);

                var medias = new List<Media>();

                if (restaurantMedia != null && restaurantMedia.Count > 0)
                {
                    medias.AddRange(restaurantMedia);
                }

                if (menuRestaurantMedia != null && menuRestaurantMedia.Count > 0)
                {
                    medias.AddRange(menuRestaurantMedia);
                }

                UPLOAD_FILES = (from m in medias
                                let fileNameSplit = m.Path?.Split('/') ?? new string[] { }
                                let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                let ThumbnailUrl = !string.IsNullOrEmpty(m.Filename) ? string.Format("{0}/{1}",
                                    _context.HttpContext.Request.GetUrl(_env).Trim(),
                                    m.Path) : string.Empty
                                select new NzUploadFile
                                {
                                    name = m.Filename,
                                    systemName = m.SystemName,
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
                _logger.LogError("FoodService.Medias.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }
    }
}
