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
using Taipla.Webservice.Models.Parameters.Backend.Food;
using Taipla.Webservice.Models.Responses.Backend.Food;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class FoodService : IFoodService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        private BaseResponse response = new BaseResponse();

        public FoodService(TAIPLA_DbContext dbContext,
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

        public async Task<BaseResponse> Foods(FoodFoodsParameter param)
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

                if (param.FOOD_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสอาหารด้วย";
                    return this.response;
                }

                if (param.AUTHOR < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสชื่อผู้สร้างรายการอาหารด้วย";
                    return this.response;
                }

                var query = _dbContext.FoodCenter.AsQueryable();

                if (param.COUNTRY_ID != null)
                {
                    query = query.Where(w => w.CountryId == param.COUNTRY_ID);
                }

                if (param.CULTURE_ID != null)
                {
                    query = query.Where(w => w.CultureId == param.CULTURE_ID);
                }

                if (param.FOOD_ID != null)
                {
                    query = query.Where(w => w.FoodId == param.FOOD_ID);
                }

                if (param.AUTHOR != null)
                {
                    query = query.Where(w => w.UserId == param.AUTHOR);
                }

                if (!string.IsNullOrEmpty(param.NAME_TH))
                {
                    var NAME_TH = param.NAME_TH?.Trim().EncodeSpacialCharacters();
                    query = query.Where(w => w.NameTh == NAME_TH || w.NameTh.Contains(NAME_TH) || w.NameTh.StartsWith(NAME_TH) || w.NameTh.EndsWith(NAME_TH));
                }

                if (this._authen.User.ROLE != RoleEnum.SUPER_ADMIN.GetString() &&
                    this._authen.User.ROLE != RoleEnum.ADMIN.GetString())
                {
                    query = query.Where(w => w.UserId == this._authen.User.USER_ID);
                }


                var foods = query.ToList();

                List<FoodResponse> responseData = new List<FoodResponse>();

                if (foods != null && foods.Count < 1)
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูลอาหาร";
                    this.response.data = responseData;
                }
                else
                {
                    responseData = (from food in foods
                                    let fileNameSplit = food.Thumbnail?.Split('/') ?? new string[] { }
                                    let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                    let ThumbnailUrl = !string.IsNullOrEmpty(food.Thumbnail) ? string.Format("{0}/{1}",
                                    _context.HttpContext.Request.GetUrl(_env).Trim(),
                                    food.Thumbnail) : string.Empty
                                    orderby food.CreatedDate descending
                                    select new FoodResponse
                                    {
                                        FOOD_ID = food.FoodId,
                                        COUNTRY_ID = food.CountryId,
                                        CULTURE_ID = food.CultureId,
                                        NAME_TH = food.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        NAME_EN = food.NameEn,
                                        USER_ID = food.UserId,
                                        DESCRIPTION = food.Description?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        COOKING_FOOD = food.CookingFood?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        DIETETIC_FOOD = food.DieteticFood?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        INGREDIENT = food.Ingredient?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        CODE = food.Code,
                                        LEGEND_STATUS = food.LegendStatus,
                                        VIEWER = food.Viewer,
                                        THUMBNAIL = ThumbnailUrl,
                                        AUTHOR = "",
                                        CREATE_DATE = food.CreatedDate,
                                        UPDATE_DATE = food.UpdatedDate
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
                _logger.LogError("FoodService.Foods.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> GetFood(FoodFoodsParameter param)
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

                if (param.FOOD_ID == null || param.FOOD_ID < 1)
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
                        USER_ID = -1;
                        break;
                    case RoleEnum.POST:
                    case RoleEnum.POST_RESTAURANT:
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

                var query = _dbContext.FoodCenter.Where(w => w.FoodId == param.FOOD_ID).AsQueryable();

                if (USER_ID != -1)
                {
                    query = query.Where(w => w.UserId == USER_ID);
                }

                var food = query.FirstOrDefault();

                if (food == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลอาหาร";
                    return this.response;
                }

                var fileNameSplit = food.Thumbnail?.Split('/') ?? new string[] { };
                string fileName = fileNameSplit?.LastOrDefault() ?? string.Empty;
                string ThumbnailUrl = !string.IsNullOrEmpty(food.Thumbnail) ? string.Format("{0}/{1}",
                            _context.HttpContext.Request.GetUrl(_env).Trim(),
                            food.Thumbnail) : string.Empty;

                var responData = new FoodResponse
                {
                    FOOD_ID = food.FoodId,
                    COUNTRY_ID = food.CountryId,
                    CULTURE_ID = food.CultureId,
                    COUNTRY_NAME_TH = "",
                    CULTURE_NAME_TH = "",
                    NAME_TH = food.NameTh?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    NAME_EN = food.NameEn,
                    USER_ID = food.UserId,
                    DESCRIPTION = food.Description?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    COOKING_FOOD = food.CookingFood?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    DIETETIC_FOOD = food.DieteticFood?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    INGREDIENT = food.Ingredient?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    CODE = food.Code,
                    LEGEND_STATUS = food.LegendStatus,
                    VIEWER = food.Viewer,
                    THUMBNAIL = food.Thumbnail,
                    AUTHOR = "",
                    CREATE_DATE = food.CreatedDate,
                    UPDATE_DATE = food.UpdatedDate,

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
                _logger.LogError("FoodService.GetFood.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Created(FoodCreatedParameter param)
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

                var NAME_TH = param.NAME_TH.Trim().EncodeSpacialCharacters() ?? string.Empty;

                var food = _dbContext.FoodCenter.Where(w => w.NameTh == NAME_TH &&
                    w.CountryId == country.CountryId &&
                    w.CultureId == culture.CultureId
                )
                    .FirstOrDefault();

                if (food != null)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "สำเร็จ, มีรายการในระบบแล้ว";
                    return this.response;
                }

                var now = DateTime.Now;

                var CODE = $"{now.ToString("yyyyMMddHHmmssffff")}_" + Guid.NewGuid().ToString("N").Substring(0, int.Parse(_configuration["Application:MaxCodeLength"].ToString())); ;

                FoodCenter foodCenter = new FoodCenter
                {
                    CountryId = param.COUNTRY_ID.Value,
                    CultureId = param.CULTURE_ID.Value,
                    NameTh = NAME_TH,
                    NameEn = param.NAME_EN,
                    Description = param.DESCRIPTION?.Trim().EncodeSpacialCharacters() ?? string.Empty,
                    CookingFood = param.COOKING_FOOD?.Trim().EncodeSpacialCharacters() ?? string.Empty,
                    DieteticFood = param.DIETETIC_FOOD?.Trim().EncodeSpacialCharacters() ?? string.Empty,
                    Ingredient = param.INGREDIENT?.Trim().EncodeSpacialCharacters() ?? string.Empty,
                    Viewer = 0,
                    Code = CODE,
                    UserId = _authen.User.USER_ID,
                    LegendStatus = !string.IsNullOrEmpty(param.LEGEND) ? 1 : 0,
                    CreatedDate = now,
                    UpdatedDate = now
                };

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.FoodCenter.Add(foodCenter);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.FOOD_CENTER.GetString(),
                            fileName,
                            foodCenter.FoodId.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.UPLOAD);

                        foodCenter.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.FoodCenter.Update(foodCenter);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.Created;
                    this.response.message = "บันทึกข้อมูลอาหารสำเร็จ";
                    this.response.data = new
                    {
                        FOOD_ID = foodCenter.FoodId,
                        CODE = foodCenter.Code
                    };
                }
                else
                {
                    _logger.LogError("FoodService.Created.Transaction.Exception: {0}", result.exception.ToString());
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("FoodService.Created.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Updated(FoodUpdatedParameter param)
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

                if (param.FOOD_ID == null || param.FOOD_ID < 1)
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

                var USER_ID = _authen.User.ROLE == RoleEnum.ADMIN.GetString() ? -1 : _authen.User.USER_ID;

                var query = _dbContext.FoodCenter.Where(w => w.FoodId == param.FOOD_ID).AsQueryable();

                if (USER_ID != -1)
                {
                    query.Where(w => w.UserId == USER_ID);
                }

                var foodCenter = query.FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลอาหาร";

                    return this.response;
                }

                var now = DateTime.Now;
                foodCenter.CountryId = param.COUNTRY_ID.Value;
                foodCenter.CultureId = param.CULTURE_ID.Value;
                foodCenter.NameTh = param.NAME_TH?.Trim().EncodeSpacialCharacters() ?? string.Empty;
                foodCenter.NameEn = param.NAME_EN;
                foodCenter.Description = param.DESCRIPTION?.Trim().EncodeSpacialCharacters() ?? string.Empty;
                foodCenter.CookingFood = param.COOKING_FOOD?.Trim().EncodeSpacialCharacters() ?? string.Empty;
                foodCenter.DieteticFood = param.DIETETIC_FOOD?.Trim().EncodeSpacialCharacters() ?? string.Empty;
                foodCenter.Ingredient = param.INGREDIENT?.Trim().EncodeSpacialCharacters() ?? string.Empty;
                foodCenter.UserId = _authen.User.USER_ID;
                foodCenter.UpdatedDate = now;
                foodCenter.LegendStatus = !string.IsNullOrEmpty(param.LEGEND) ? 1 : 0;

                Media upload = null;
                Media oldUpload = _dbContext.UploadFileUtilities.GetMedia(
                     UploadEnum.FOOD_CENTER.GetString()
                     , foodCenter.FoodId.ToString()).CopyTo<Media, Media>();

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.FoodCenter.Update(foodCenter);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.FOOD_CENTER.GetString(),
                            foodCenter.FoodId.ToString());

                        if (upload == null)
                        {
                            upload = _dbContext.UploadFileUtilities.CreateMedia(
                                 UploadEnum.FOOD_CENTER.GetString(),
                                 fileName,
                                 foodCenter.FoodId.ToString(),
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

                        foodCenter.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.FoodCenter.Update(foodCenter);
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
                    this.response.message = "แก้ไขข้อมูลอาหารสำเร็จ";
                    this.response.data = new
                    {
                        FOOD_ID = foodCenter.FoodId
                    };
                }
                else
                {
                    _logger.LogError("FoodService.Updated.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("FoodService.Updated.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Deleted(FoodDeletedParameter param)
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

                if (param.FOOD_ID == null || param.FOOD_ID < 1)
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

                var foodCenter = _dbContext.FoodCenter.Where(w => w.FoodId == param.FOOD_ID).FirstOrDefault();

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลอาหาร";

                    return this.response;
                }

                if (foodCenter.UserId != _authen.User.USER_ID)
                {
                    if (_authen.User.ROLE != RoleEnum.ADMIN.GetString() && _authen.User.ROLE != RoleEnum.SUPER_ADMIN.GetString())
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่มีสิทธิ์ลบข้อมูล";

                        return this.response;
                    }
                }

                var media = _dbContext.UploadFileUtilities.DeleteMedia(foodCenter.Thumbnail);

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.FoodCenter.Remove(foodCenter);
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
                    _dbContext.UploadFileUtilities.RemoveFolder(foodCenter.Thumbnail);
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ลบข้อมูลอาหารสำเร็จ";
                    this.response.data = new
                    {
                        FOOD_ID = foodCenter.FoodId
                    };
                }
                else
                {
                    _logger.LogError("FoodService.Deleted.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("FoodService.Deleted.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        //=>new 

        public async Task<BaseResponse> Medias(FoodFoodsParameter param)
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

                if (param.FOOD_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสอาหารด้วย";
                    return this.response;
                }

                var foodCenter = _dbContext.FoodCenter.FirstOrDefault(f => f.FoodId == param.FOOD_ID &&
                    f.CountryId == param.COUNTRY_ID &&
                    f.CultureId == param.CULTURE_ID);

                if (foodCenter == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลอาหาร";

                    return this.response;
                }

                var medias = _dbContext.UploadFileUtilities.GetMedias(UploadEnum.FOOD_CENTER.GetString()
                    , foodCenter.FoodId.ToString()
                    , foodCenter.Thumbnail);


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
                _logger.LogError("FoodService.Medias.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }
    }
}
