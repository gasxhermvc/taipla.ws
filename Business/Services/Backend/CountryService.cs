using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
using Taipla.Webservice.Models.Parameters.Backend.Country;
using Taipla.Webservice.Models.Responses.Backend.Country;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
namespace Taipla.Webservice.Business.Services.Backend
{
    public class CountryService : ICountryService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;
        private BaseResponse response = new BaseResponse();

        public CountryService(TAIPLA_DbContext dbContext,
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

        public async Task<BaseResponse> Countries(CountryCountriesParameter param)
        {
            try
            {
                var query = _dbContext.FoodCountry.AsQueryable();

                if (param.COUNTRY_ID != null)
                {
                    query.Where(w => w.CountryId == param.COUNTRY_ID);
                }

                var countries = query.ToList();

                List<CountryResponse> responseData = new List<CountryResponse>();

                if (countries != null && countries.Count < 1)
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูลประเทศ";
                    this.response.data = responseData;
                }
                else
                {
                    responseData = (from country in countries
                                    let fileNameSplit = country.Thumbnail?.Split('/') ?? new string[] { }
                                    let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                    let ThumbnailUrl = !string.IsNullOrEmpty(country.Thumbnail) ? string.Format("{0}/{1}",
                                    _context.HttpContext.Request.GetUrl(_env).Trim(),
                                    country.Thumbnail) : string.Empty
                                    orderby country.CreatedDate descending
                                    select new CountryResponse
                                    {
                                        COUNTRY_ID = country.CountryId,
                                        NAME_TH = country.NameTh,
                                        NAME_EN = country.NameEn,
                                        USER_ID = country.UserId,
                                        DESCRIPTION = country.Description,
                                        THUMBNAIL = ThumbnailUrl,
                                        AUTHOR = "",
                                        CREATE_DATE = country.CreatedDate,
                                        UPDATE_DATE = country.UpdatedDate
                                    }).ToList();

                    if (responseData.Count > 0)
                    {
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
                    this.response.message = "ดึงข้อมูลประเทศสำเร็จ";
                    this.response.total = responseData.Count;
                    this.response.data = responseData;
                }

                this.response.success = true;
            }
            catch (Exception e)
            {
                _logger.LogError("CountryService.Countries.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }


            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> GetCountry(CountryCountriesParameter param)
        {
            try
            {
                if (param.COUNTRY_ID == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";
                    return this.response;
                }

                var USER_ID = _authen.User.ROLE == RoleEnum.ADMIN.GetString() ? -1 : _authen.User.USER_ID;

                var query = _dbContext.FoodCountry.Where(w =>
                    w.CountryId == param.COUNTRY_ID).AsQueryable();

                if (USER_ID != -1)
                {
                    query.Where(w => w.UserId == USER_ID);
                }

                var country = query.FirstOrDefault();

                if (country == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";
                    return this.response;
                }


                var fileNameSplit = country.Thumbnail?.Split('/') ?? new string[] { };
                string fileName = fileNameSplit?.LastOrDefault() ?? string.Empty;
                string ThumbnailUrl = !string.IsNullOrEmpty(country.Thumbnail) ? string.Format("{0}/{1}",
                            _context.HttpContext.Request.GetUrl(_env).Trim(),
                            country.Thumbnail) : string.Empty;


                var responData = new CountryResponse
                {
                    COUNTRY_ID = country.CountryId,
                    NAME_TH = country.NameTh,
                    NAME_EN = country.NameEn,
                    USER_ID = country.UserId,
                    DESCRIPTION = country.Description,
                    THUMBNAIL = ThumbnailUrl,
                    AUTHOR = "",
                    CREATE_DATE = country.CreatedDate,
                    UPDATE_DATE = country.UpdatedDate,

                    UPLOAD_FILES = (!string.IsNullOrEmpty(ThumbnailUrl) ? new List<NzUploadFile> {
                        new NzUploadFile
                        {
                            name = fileName,
                            url = ThumbnailUrl,
                        } } : new List<NzUploadFile>())
                };

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = "ดึงข้อมูลประเทศสำเร็จ";
                this.response.data = responData;
            }
            catch (Exception e)
            {
                _logger.LogError("CountryService.GetCountry.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Created(CountryCreatedParameter param)
        {
            try
            {
                var country = _dbContext.FoodCountry.Where(w => w.NameTh == param.NAME_TH)
                    .FirstOrDefault();

                if (country != null)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "สำเร็จ, มีรายการในระบบแล้ว";
                    return this.response;
                }

                var now = DateTime.Now;

                FoodCountry foodCountry = new FoodCountry
                {
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
                    _dbContext.FoodCountry.Add(foodCountry);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.FOOD_COUNTRY.GetString(),
                            fileName,
                            foodCountry.CountryId.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.UPLOAD);

                        foodCountry.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.FoodCountry.Update(foodCountry);
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
                        COUNTRY_ID = foodCountry.CountryId
                    };
                }
                else
                {
                    _logger.LogError("CountryService.Created.Transaction.Exception: {0}", result.exception.ToString());
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("CountryService.Created.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Updated(CountryUpdatedParameter param)
        {
            try
            {
                if (param.COUNTRY_ID == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";
                    return this.response;
                }

                var foodCountry = _dbContext.FoodCountry.FirstOrDefault(f =>
                    f.CountryId == param.COUNTRY_ID);

                if (foodCountry == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";

                    return this.response;
                }

                var now = DateTime.Now;

                foodCountry.NameTh = param.NAME_TH;
                foodCountry.NameEn = param.NAME_EN;
                foodCountry.Description = param.DESCRIPTION;
                foodCountry.UserId = _authen.User.USER_ID;
                foodCountry.UpdatedDate = now;

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.FoodCountry.Update(foodCountry);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.FOOD_COUNTRY.GetString(),
                            foodCountry.CountryId.ToString());

                        if (upload == null)
                        {
                            upload = _dbContext.UploadFileUtilities.CreateMedia(
                                 UploadEnum.FOOD_COUNTRY.GetString(),
                                 fileName,
                                 foodCountry.CountryId.ToString(),
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

                        foodCountry.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.FoodCountry.Update(foodCountry);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "แก้ไขข้อมูลประเทศสำเร็จ";
                    this.response.data = new
                    {
                        COUNTRY_ID = foodCountry.CountryId
                    };
                }
                else
                {
                    _logger.LogError("CountryService.Updated.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("CountryService.Updated.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Deleted(CountryDeletedParameter param)
        {
            try
            {
                if (param.COUNTRY_ID == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";
                    return this.response;
                }

                var foodCountry = _dbContext.FoodCountry.FirstOrDefault(f =>
                    f.CountryId == param.COUNTRY_ID);

                if (foodCountry == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลประเทศ";

                    return this.response;
                }

                if (_authen.User.ROLE != RoleEnum.ADMIN.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "ไม่มีสิทธิ์ลบข้อมูล";

                    return this.response;
                }

                var media = _dbContext.UploadFileUtilities.DeleteMedia(foodCountry.Thumbnail);

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.FoodCountry.Remove(foodCountry);
                    if (media != null)
                    {
                        _dbContext.Media.Remove(media);
                        //=>ต้องการลบไฟล์ด้วย
                        //_dbContext.UploadFileUtilities.RemoveFile(media);
                        //_dbContext.UploadFileUtilities.RemoveFolder(foodCountry.Thumbnail);
                    }
                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    _dbContext.UploadFileUtilities.RemoveFolder(foodCountry.Thumbnail);
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ลบข้อมูลประเทศสำเร็จ";
                    this.response.data = new
                    {
                        COUNTRY_ID = foodCountry.CountryId
                    };
                }
                else
                {
                    _logger.LogError("CountryService.Deleted.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("CountryService.Deleted.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

    }
}
