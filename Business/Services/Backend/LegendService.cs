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
using Taipla.Webservice.Models.Parameters.Backend.Legend;
using Taipla.Webservice.Models.Responses.Backend.Legend;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class LegendService : ILegendService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        private BaseResponse response = new BaseResponse();

        public LegendService(TAIPLA_DbContext dbContext,
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

        public async Task<BaseResponse> Legends(LegendLegendsParameter param)
        {
            try
            {
                if (param.LEGEND_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, รหัสตำนานไม่ถูกต้อง";
                    return this.response;
                }

                var query = _dbContext.Legend.AsQueryable();

                if (param.LEGEND_ID != null)
                {
                    query = query.Where(w => w.Id == param.LEGEND_ID);
                }

                if (!string.IsNullOrEmpty(param.CODE))
                {
                    query = query.Where(w => w.Code == param.CODE);
                }

                var legends = query.ToList();

                List<LegendResponse> responseData = new List<LegendResponse>();

                if (legends != null && legends.Count < 1)
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูลอาหาร";
                    this.response.data = responseData;
                }
                else
                {
                    responseData = (from legend in legends
                                    let fileNameSplit = legend.Thumbnail?.Split('/') ?? new string[] { }
                                    let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                    let ThumbnailUrl = !string.IsNullOrEmpty(legend.Thumbnail) ? string.Format("{0}/{1}",
                                    _context.HttpContext.Request.GetUrl(_env).Trim(),
                                    legend.Thumbnail) : string.Empty
                                    orderby legend.CreatedDate descending
                                    select new LegendResponse
                                    {
                                        LEGEND_ID = legend.Id,
                                        DESCRIPTION = legend.Description,
                                        CODE = legend.Code,
                                        LEGEND_TYPE = legend.LegendType.ToString(),
                                        THUMBNAIL = ThumbnailUrl,
                                        CREATED_DATE = legend.CreatedDate,
                                        UPDATED_DATE = legend.UpdatedDate
                                    }).ToList();

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลอาหารสำเร็จ";
                    this.response.total = responseData.Count;
                    this.response.data = responseData;
                }
                this.response.success = true;
            }
            catch (Exception e)
            {
                _logger.LogError("LegendService.Legends.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> GetLegend(LegendLegendsParameter param)
        {
            try
            {
                if (param.LEGEND_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, รหัสตำนานไม่ถูกต้อง";
                    return this.response;
                }

                var legendQuery = _dbContext.Legend.Where(w => w.Code == param.CODE).AsQueryable();

                if (param.LEGEND_ID > 1)
                {
                    legendQuery = legendQuery.Where(w => w.Id == param.LEGEND_ID);
                }

                var legend = legendQuery.FirstOrDefault();

                if (legend == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลตำนาน";
                    return this.response;
                }

                var fileNameSplit = legend.Thumbnail?.Split('/') ?? new string[] { };
                string fileName = fileNameSplit?.LastOrDefault() ?? string.Empty;
                string ThumbnailUrl = !string.IsNullOrEmpty(legend.Thumbnail) ? string.Format("{0}/{1}",
                            _context.HttpContext.Request.GetUrl(_env).Trim(),
                            legend.Thumbnail) : string.Empty;

                var responData = new LegendResponse
                {
                    LEGEND_ID = legend.Id,
                    DESCRIPTION = legend.Description,
                    CODE = legend.Code,
                    LEGEND_TYPE = legend.LegendType.ToString(),
                    THUMBNAIL = ThumbnailUrl,
                    CREATED_DATE = legend.CreatedDate,
                    UPDATED_DATE = legend.UpdatedDate,

                    UPLOAD_FILES = (!string.IsNullOrEmpty(ThumbnailUrl) ? new List<NzUploadFile> {
                        new NzUploadFile
                        {
                            name = fileName,
                            url = ThumbnailUrl,
                        } } : new List<NzUploadFile>())
                };

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = "ดึงข้อมูลตำนานสำเร็จ";
                this.response.data = responData;
            }
            catch (Exception e)
            {
                _logger.LogError("LegendService.GetLegend.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Created(LegendCreatedParameter param)
        {
            try
            {
                if (string.IsNullOrEmpty(param.CODE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ไม่พบรหัสยืนยันตัวตนของต้นทาง";
                    return this.response;
                }

                string Code = string.Empty;
                FoodCenter foodCenter = null;
                RestaurantMenu restaurant = null;
                if (param.LEGEND_TYPE == LegendEnum.LEGEND_FOOD)
                {
                    foodCenter = _dbContext.FoodCenter.Where(w => w.Code == param.CODE)
                        .FirstOrDefault();

                    if (foodCenter != null)
                    {
                        Code = foodCenter.Code;
                    }
                }

                if (param.LEGEND_TYPE == LegendEnum.LEGEND_RESTAURANT)
                {
                    restaurant = _dbContext.RestaurantMenu.Where(w => w.Code == param.CODE)
                       .FirstOrDefault();

                    if (restaurant != null)
                    {
                        Code = restaurant.Code;
                    }
                }

                if (string.IsNullOrEmpty(Code))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, ไม่พบรหัสเชื่อมโยงของตำนาน";
                    return this.response;
                }

                var now = DateTime.Now;

                Legend legend = new Legend
                {
                    Description = param.DESCRIPTION?.Trim() ?? string.Empty,
                    Code = param.CODE,
                    LegendType = (sbyte)param.LEGEND_TYPE,
                    CreatedDate = now,
                    UpdatedDate = now
                };

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    if (foodCenter != null)
                    {
                        foodCenter.LegendStatus = 1;
                        _dbContext.FoodCenter.Update(foodCenter);
                    }

                    if (restaurant != null)
                    {
                        restaurant.LegendStatus = 1;
                        _dbContext.RestaurantMenu.Update(restaurant);
                    }

                    _dbContext.Legend.Add(legend);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.LEGEND.GetString(),
                            fileName,
                            legend.Id.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.UPLOAD);

                        legend.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.Legend.Update(legend);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.Created;
                    this.response.message = "บันทึกข้อมูลตำนานสำเร็จ";
                    this.response.data = new
                    {
                        LEGEND_ID = legend.Id
                    };
                }
                else
                {
                    _logger.LogError("LegendService.Created.Transaction.Exception: {0}", result.exception.ToString());
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("LegendService.Created.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Updated(LegendUpdatedParameter param)
        {
            try
            {
                if (string.IsNullOrEmpty(param.CODE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ไม่พบรหัสยืนยันตัวตนของต้นทาง";
                    return this.response;
                }
                string Code = string.Empty;
                FoodCenter foodCenter = null;
                RestaurantMenu restaurant = null;

                if (param.LEGEND_TYPE == LegendEnum.LEGEND_FOOD)
                {
                    foodCenter = _dbContext.FoodCenter.Where(w => w.Code == param.CODE)
                       .FirstOrDefault();

                    if (foodCenter != null)
                    {
                        Code = foodCenter.Code;
                    }
                }

                if (param.LEGEND_TYPE == LegendEnum.LEGEND_RESTAURANT)
                {
                    restaurant = _dbContext.RestaurantMenu.Where(w => w.Code == param.CODE)
                        .FirstOrDefault();

                    if (restaurant != null)
                    {
                        Code = restaurant.Code;
                    }
                }

                if (string.IsNullOrEmpty(Code))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, ไม่พบรหัสเชื่อมโยงของตำนาน";
                    return this.response;
                }


                var legend = _dbContext.Legend.Where(w => w.Id == param.LEGEND_ID && w.Code == param.CODE).FirstOrDefault();

                if (legend == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลตำนาน";

                    return this.response;
                }

                var now = DateTime.Now;
                legend.Description = param.DESCRIPTION;
                legend.UpdatedDate = now;

                Media upload = null;
                Media oldUpload = _dbContext.UploadFileUtilities.GetMedia(
                    UploadEnum.LEGEND.GetString()
                    , legend.Id.ToString()).CopyTo<Media, Media>();

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    if (foodCenter != null)
                    {
                        foodCenter.LegendStatus = 1;
                        _dbContext.FoodCenter.Update(foodCenter);
                    }

                    if (restaurant != null)
                    {
                        restaurant.LegendStatus = 1;
                        _dbContext.RestaurantMenu.Update(restaurant);
                    }

                    _dbContext.Legend.Update(legend);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.LEGEND.GetString(),
                            legend.Id.ToString());

                        if (upload == null)
                        {
                            upload = _dbContext.UploadFileUtilities.CreateMedia(
                                 UploadEnum.LEGEND.GetString(),
                                 fileName,
                                 legend.Id.ToString(),
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

                        legend.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.Legend.Update(legend);
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
                    this.response.message = "แก้ไขข้อมูลตำนานสำเร็จ";
                    this.response.data = new
                    {
                        LEGEND_ID = legend.Id
                    };
                }
                else
                {
                    _logger.LogError("LegendService.Updated.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("LegendService.Updated.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Deleted(LegendDeletedParameter param)
        {
            try
            {
                if (param.LEGEND_ID == null || param.LEGEND_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสตำนานด้วย";
                    return this.response;
                }

                if (string.IsNullOrEmpty(param.CODE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสเชื่อมโยงตำนานด้วย";
                    return this.response;
                }

                string Code = string.Empty;
                FoodCenter foodCenter = null;
                RestaurantMenu restaurant = null;

                if (param.LEGEND_TYPE == LegendEnum.LEGEND_FOOD)
                {
                    foodCenter = _dbContext.FoodCenter.Where(w => w.Code == param.CODE)
                       .FirstOrDefault();

                    if (foodCenter != null)
                    {
                        Code = foodCenter.Code;
                    }
                }

                if (param.LEGEND_TYPE == LegendEnum.LEGEND_RESTAURANT)
                {
                    restaurant = _dbContext.RestaurantMenu.Where(w => w.Code == param.CODE)
                        .FirstOrDefault();

                    if (restaurant != null)
                    {
                        Code = restaurant.Code;
                    }
                }

                if (string.IsNullOrEmpty(Code))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, ไม่พบรหัสเชื่อมโยงของตำนาน";
                    return this.response;
                }

                var legend = _dbContext.Legend.Where(w => w.Id == param.LEGEND_ID).FirstOrDefault();

                if (legend == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลตำนาน";

                    return this.response;
                }


                Media media = null;

                if (foodCenter != null)
                {
                    media = _dbContext.UploadFileUtilities.DeleteMedia(foodCenter.Thumbnail);
                }


                if (restaurant != null)
                {
                    media = _dbContext.UploadFileUtilities.DeleteMedia(restaurant.Thumbnail);
                }

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    if (foodCenter != null)
                    {
                        foodCenter.LegendStatus = 0;
                        _dbContext.FoodCenter.Update(foodCenter);
                    }

                    if (restaurant != null)
                    {
                        restaurant.LegendStatus = 0;
                        _dbContext.RestaurantMenu.Update(restaurant);
                    }

                    _dbContext.Legend.Remove(legend);
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
                    _dbContext.UploadFileUtilities.RemoveFolder(legend.Thumbnail);
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ลบข้อมูลตำนานสำเร็จ";
                    this.response.data = new
                    {
                        LEGEND_ID = legend.Id
                    };
                }
                else
                {
                    _logger.LogError("LegendService.Deleted.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("LegendService.Deleted.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }
    }
}
