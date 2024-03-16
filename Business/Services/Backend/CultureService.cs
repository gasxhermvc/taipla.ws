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
using Taipla.Webservice.Models.Parameters.Backend.Culture;
using Taipla.Webservice.Models.Responses.Backend.Culture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class CultureService : ICultureService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        private BaseResponse response = new BaseResponse();

        public CultureService(TAIPLA_DbContext dbContext,
            IAuthenticationService<UserInfo> authen,
            IHttpContextAccessor context,
            ILoggerFactory logger,
            IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _authen = authen;
            _context = context;
            _logger = logger.CreateLogger("Service");
            _env = env;
        }

        public async Task<BaseResponse> Cultures(CultureCulturesParameter param)
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

                var query = _dbContext.FoodCulture.AsQueryable();

                if (param.COUNTRY_ID != null)
                {
                    query.Where(w => w.CountryId == param.COUNTRY_ID);
                }

                if (param.CULTURE_ID != null)
                {
                    query.Where(w => w.CultureId == param.CULTURE_ID);
                }

                var cultures = query.ToList();

                List<CultureResponse> responseData = new List<CultureResponse>();

                if (cultures != null && cultures.Count < 1)
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูลวัฒนธรรมอาหาร";
                    this.response.data = responseData;
                }
                else
                {
                    responseData = (from culture in cultures
                                    let fileNameSplit = culture.Thumbnail?.Split('/') ?? new string[] { }
                                    let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                    let ThumbnailUrl = !string.IsNullOrEmpty(culture.Thumbnail) ? string.Format("{0}/{1}",
                                    _context.HttpContext.Request.GetUrl(_env).Trim(),
                                    culture.Thumbnail) : string.Empty

                                    orderby culture.CreatedDate descending
                                    select new CultureResponse
                                    {
                                        CULTURE_ID = culture.CultureId,
                                        COUNTRY_ID = culture.CountryId,
                                        COUNTRY_NAME_TH = "",
                                        NAME_TH = culture.NameTh,
                                        NAME_EN = culture.NameEn,
                                        USER_ID = culture.UserId,
                                        DESCRIPTION = culture.Description,
                                        THUMBNAIL = ThumbnailUrl,
                                        AUTHOR = "",
                                        CREATE_DATE = culture.CreatedDate,
                                        UPDATE_DATE = culture.UpdatedDate
                                    }).ToList();

                    if (responseData.Count > 0)
                    {
                        var countryIds = responseData.Select(s => s.COUNTRY_ID).ToList();
                        var countries = _dbContext.CountryUtilities.GetCountries((FoodCountry country) => new
                        {
                            COUNTRY_ID = country.CountryId,
                            NAME_TH = country.NameTh
                        }, countryIds)
                        ;
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
                    }

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลวัฒนธรรมอาหารสำเร็จ";
                    this.response.total = responseData.Count;
                    this.response.data = responseData;
                }

                this.response.success = true;
            }
            catch (Exception e)
            {
                _logger.LogError("CultureService.Cultures.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> GetCulture(CultureCulturesParameter param)
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

                var USER_ID = _authen.User.ROLE == RoleEnum.ADMIN.GetString() ? -1 : _authen.User.USER_ID;

                var query = _dbContext.FoodCulture.Where(w =>
                    w.CultureId == param.CULTURE_ID).AsQueryable();

                if (USER_ID != -1)
                {
                    query.Where(w => w.UserId == USER_ID);
                }

                var culture = query.FirstOrDefault();

                if (culture == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลวัฒนธรรมอาหาร";
                    return this.response;
                }


                var fileNameSplit = culture.Thumbnail?.Split('/') ?? new string[] { };
                string fileName = fileNameSplit?.LastOrDefault() ?? string.Empty;
                string ThumbnailUrl = !string.IsNullOrEmpty(culture.Thumbnail) ? string.Format("{0}/{1}",
                            _context.HttpContext.Request.GetUrl(_env).Trim(),
                            culture.Thumbnail) : string.Empty;


                var responData = new CultureResponse
                {
                    CULTURE_ID = culture.CultureId,
                    COUNTRY_ID = culture.CountryId,
                    COUNTRY_NAME_TH = "",
                    NAME_TH = culture.NameTh,
                    NAME_EN = culture.NameEn,
                    USER_ID = culture.UserId,
                    DESCRIPTION = culture.Description,
                    THUMBNAIL = ThumbnailUrl,
                    AUTHOR = "",
                    CREATE_DATE = culture.CreatedDate,
                    UPDATE_DATE = culture.UpdatedDate,

                    UPLOAD_FILES = (!string.IsNullOrEmpty(ThumbnailUrl) ? new List<NzUploadFile> {
                        new NzUploadFile
                        {
                            name = fileName,
                            url = ThumbnailUrl,
                        } } : new List<NzUploadFile>())
                };

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = "ดึงข้อมูลวัฒนธรรมอาหารสำเร็จ";
                this.response.data = responData;
            }
            catch (Exception e)
            {
                _logger.LogError("CultureService.GetCountry.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Created(CultureCreatedParameter param)
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

                var culture = _dbContext.FoodCulture.Where(w => w.CountryId == param.COUNTRY_ID && w.NameTh == param.NAME_TH)
                    .FirstOrDefault();

                if (culture != null)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "สำเร็จ, มีรายการในระบบแล้ว";
                    return this.response;
                }

                var now = DateTime.Now;

                FoodCulture foodCulture = new FoodCulture
                {
                    CountryId = param.COUNTRY_ID.Value,
                    NameTh = param.NAME_TH,
                    NameEn = param.NAME_EN,
                    Description = param.DESCRIPTION,
                    UserId = _authen.User.USER_ID,
                    CreatedDate = now,
                    UpdatedDate = now
                };

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.FoodCulture.Add(foodCulture);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.FOOD_CULTURE.GetString(),
                            fileName,
                            foodCulture.CultureId.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.UPLOAD);

                        foodCulture.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.FoodCulture.Update(foodCulture);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.Created;
                    this.response.message = "บันทึกข้อมูลประเทศสำเร็จ";
                    this.response.data = new
                    {
                        CULTURE_ID = foodCulture.CultureId
                    };
                }
                else
                {
                    _logger.LogError("CultureService.Created.Transaction.Exception: {0}", result.exception.ToString());
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("CultureService.Created.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Updated(CultureUpdatedParameter param)
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

                var country = _dbContext.FoodCountry.FirstOrDefault(f =>
                    f.CountryId == param.COUNTRY_ID);

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";

                    return this.response;
                }

                var foodCulture = _dbContext.FoodCulture.FirstOrDefault(f =>
                    f.CultureId == param.CULTURE_ID);

                if (foodCulture == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";

                    return this.response;
                }

                var now = DateTime.Now;

                foodCulture.CountryId = param.COUNTRY_ID.Value;
                foodCulture.NameTh = param.NAME_TH;
                foodCulture.NameEn = param.NAME_EN;
                foodCulture.Description = param.DESCRIPTION;
                foodCulture.UserId = _authen.User.USER_ID;
                foodCulture.UpdatedDate = now;

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.FoodCulture.Update(foodCulture);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.FOOD_CULTURE.GetString(),
                            foodCulture.CultureId.ToString());

                        if (upload == null)
                        {
                            upload = _dbContext.UploadFileUtilities.CreateMedia(
                                 UploadEnum.FOOD_CULTURE.GetString(),
                                 fileName,
                                 foodCulture.CultureId.ToString(),
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

                        foodCulture.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.FoodCulture.Update(foodCulture);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "แก้ไขข้อมูลวัฒนธรรมอาหารสำเร็จ";
                    this.response.data = new
                    {
                        CULTURE_ID = foodCulture.CultureId
                    };
                }
                else
                {
                    _logger.LogError("CultureService.Updated.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("CultureService.Updated.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Deleted(CultureDeletedParameter param)
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

                var country = _dbContext.FoodCountry.FirstOrDefault(f =>
                    f.CountryId == param.COUNTRY_ID);

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";

                    return this.response;
                }

                var foodCulture = _dbContext.FoodCulture.FirstOrDefault(f =>
                     f.CultureId == param.CULTURE_ID &&
                     f.CountryId == param.COUNTRY_ID);

                if (foodCulture == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลวัฒนธรรมอาหาร";

                    return this.response;
                }

                if (_authen.User.ROLE != RoleEnum.ADMIN.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "ไม่มีสิทธิ์ลบข้อมูล";

                    return this.response;
                }

                var media = _dbContext.UploadFileUtilities.DeleteMedia(foodCulture.Thumbnail);

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.FoodCulture.Remove(foodCulture);
                    if (media != null)
                    {
                        _dbContext.Media.Remove(media);
                        //=>ต้องการลบไฟล์ด้วย
                        //_dbContext.UploadFileUtilities.RemoveFile(media);
                        // _dbContext.UploadFileUtilities.RemoveFolder(foodCulture.Thumbnail);
                    }
                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    _dbContext.UploadFileUtilities.RemoveFolder(foodCulture.Thumbnail);
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ลบข้อมูลวัฒนธรรมอาหารสำเร็จ";
                    this.response.data = new
                    {
                        CULTURE_ID = foodCulture.CultureId
                    };
                }
                else
                {
                    _logger.LogError("CultureService.Deleted.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("CultureService.Deleted.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }
    }
}
