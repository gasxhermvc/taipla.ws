using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
using Taipla.Webservice.Models.Parameters.Backend.Media;
using Taipla.Webservice.Models.Responses.Backend.Media;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class MediaService : IMediaService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        private BaseResponse response = new BaseResponse();

        public MediaService(TAIPLA_DbContext dbContext
            , IConfiguration configuration
            , IHttpContextAccessor context
            , ILoggerFactory loggerFactory
            , IAuthenticationService<UserInfo> authen
            , IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _context = context;
            _authen = authen;
            _env = env;
            _logger = loggerFactory.CreateLogger("Service");
        }

        public async Task<BaseResponse> Medias(MediaParameter param)
        {
            List<MediaResponse> response = new List<MediaResponse>();
            try
            {
                var mediaQuery = _dbContext.Media.Where(w => !string.IsNullOrEmpty(w.Path)).AsQueryable();

                if (param.PATH != null && !ValidateExtension.isUploadTypeValid((UploadEnum)param.PATH))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ประเภทอัพโหลดไม่ถูกต้อง";
                    return this.response;
                }

                if (param.PATH != null)
                {
                    mediaQuery = mediaQuery.Where(w => w.SystemName == param.PATH.Value.GetString());
                }

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                List<string> userIds = null;
                string resId = string.Empty;
                int restaurantId;
                List<dynamic> resMenus = null;
                List<string> resMenuIds = null;
                List<string> codes = null;
                List<string> legendIds = null;

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                        break;
                    case RoleEnum.OWNER:
                        //=>USER_ID ของ Staff และตัวเอง
                        userIds = _dbContext.User.Where(w => w.UserId == _authen.User.USER_ID ||
                           (w.ParentId == _authen.User.USER_ID && w.Role == RoleEnum.STAFF.GetString()))
                            .Select(s => s.UserId)
                            .Distinct()
                            .Select(s => s.ToString())
                            .ToList();

                        //=>ร้านอาหาร
                        resId = _dbContext.Restaurant.Where(w => w.OwnerId == _authen.User.USER_ID)
                           .Select(s => s.ResId.ToString())
                           .FirstOrDefault();

                        //=>เมนูอาหาร
                        restaurantId = int.Parse(resId);
                        resMenus = _dbContext.RestaurantMenu.Where(w => w.ResId == restaurantId)
                           .Select(s => new
                           {
                               MenuId = s.MenuId,
                               Code = s.Code
                           } as dynamic)
                           .Distinct()
                           .ToList();

                        resMenuIds = resMenus
                           .Select(s => ((dynamic)s).MenuId as string)
                           .ToList();

                        //=>ตำนานอาหาร
                        codes = resMenus
                            .Select(s => ((dynamic)s).Code as string)
                            .ToList();
                        legendIds = _dbContext.Legend.Where(w => codes.Contains(w.Code))
                           .Select(s => s.Id.ToString())
                           .ToList();

                        mediaQuery = mediaQuery.Where(w =>
                            (userIds.Contains(w.RefId) && w.SystemName == UploadEnum.UM.GetString()) ||
                            (w.RefId == resId && w.SystemName == UploadEnum.RESTAURANT.GetString()) ||
                            (resMenuIds.Contains(w.RefId) && w.SystemName == UploadEnum.RESTAURANT_MENU.GetString()) ||
                            (legendIds.Contains(w.RefId) && w.SystemName == UploadEnum.LEGEND.GetString()));
                        break;
                    case RoleEnum.STAFF:
                        //=>ชื่อเจ้าของร้าน
                        var user = _dbContext.User.FirstOrDefault(f => f.UserId == _authen.User.USER_ID);

                        //=>ชื่อผู้ใช้งานภายในร้าน
                        userIds = _dbContext.User.Where(w => w.ParentId == user.ParentId)
                           .Select(s => s.UserId)
                           .Distinct()
                           .Select(s => s.ToString())
                           .ToList();

                        //=>ร้านอาหาร
                        resId = _dbContext.Restaurant.Where(w => w.OwnerId == user.ParentId)
                           .Select(s => s.ResId.ToString())
                           .FirstOrDefault();

                        //=>เมนูอาหาร
                        restaurantId = int.Parse(resId);
                        resMenus = _dbContext.RestaurantMenu.Where(w => w.ResId == restaurantId)
                           .Select(s => new
                           {
                               MenuId = s.MenuId,
                               Code = s.Code
                           } as dynamic)
                           .Distinct()
                           .ToList();

                        resMenuIds = resMenus
                            .Select(s => ((dynamic)s).MenuId as string)
                            .ToList();

                        //=>ตำนานอาหาร
                        codes = resMenus
                            .Select(s => ((dynamic)s).Code as string)
                            .ToList();
                        legendIds = _dbContext.Legend.Where(w => codes.Contains(w.Code))
                           .Select(s => s.Id.ToString())
                           .ToList();

                        mediaQuery = mediaQuery.Where(w =>
                            (userIds.Contains(w.RefId) && w.SystemName == UploadEnum.UM.GetString()) ||
                            (w.RefId == resId && w.SystemName == UploadEnum.RESTAURANT.GetString()) ||
                            (resMenuIds.Contains(w.RefId) && w.SystemName == UploadEnum.RESTAURANT_MENU.GetString()) ||
                            (legendIds.Contains(w.RefId) && w.SystemName == UploadEnum.LEGEND.GetString()));
                        break;
                    default:
                        break;
                }

                var medias = mediaQuery.ToList();

                if (medias != null && medias.Count > 0)
                {
                    response = (from m in medias
                                let system = m.SystemName.ParseEnum<UploadEnum>(UploadEnum.UNKNOW)
                                let image = _env.GetImageThumbnail(_context, m?.Path ?? string.Empty, ImageExtension.DEFAULT_IMAGE)
                                where ValidateExtension.isUploadTypeValid(system)
                                select new MediaResponse
                                {
                                    MEDIA_ID = m.Id,
                                    FILENAME = m.Filename,
                                    SYSTEM_NAME = m.SystemName,
                                    PATH = system,
                                    REF_ID = m.RefId,
                                    URL = image.image,
                                    CREATE_DATE = m.CreatedDate,
                                    UPDATE_DATE = m.UpdatedDate
                                }).ToList();

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ, แต่ไม่พบข้อมูล";

                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ, แต่ไม่พบข้อมูล";
                }

                this.response.total = response.Count;
                this.response.data = response;
                this.response.success = true;

            }
            catch (Exception e)
            {
                _logger.LogError("MediaService.Medias.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Upload(MediaUploadParameter param)
        {
            try
            {
                if (param.Uploads.Count < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบไฟล์ที่ต้องการอัพโหลด";
                    return this.response;
                }

                List<Media> medias = new List<Media>();
                List<string> listFiles = new List<string>();
                var files = param.Uploads.Files;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {

                    foreach (var file in files)
                    {
                        var now = DateTime.Now;
                        string fileName = file.FileName;

                        var upload = _dbContext.UploadFileUtilities.CreateMedia(
                            param.SystemName.GetString(),
                            fileName,
                            param.RefId,
                            now);

                        medias.Add(upload);

                        string fullName = _dbContext.UploadFileUtilities.GetPathFile(upload);
                        listFiles.Add(fullName);
                        _dbContext.UploadFileUtilities.SaveAs(upload, file);
                    };

                    _dbContext.Media.AddRange(medias);
                    _dbContext.SaveChanges();
                });

                if (!result.success)
                {
                    foreach (var file in listFiles)
                    {
                        if (System.IO.File.Exists(file))
                        {
                            System.IO.File.Delete(file);
                        }
                    }

                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "อัพโหลดรูปภาพไม่สำเร็จ";
                    this.response.total = 0;
                    this.response.data = null;
                    this.response.success = false;
                }
                else
                {
                    this.response.total = medias.Count;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "อัพโหลดสำเร็จ";
                    this.response.data = (from m in medias
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
                    this.response.success = true;
                }

            }
            catch (Exception e)
            {
                _logger.LogError("MediaService.Upload.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> RemoveUpload(MediaRemoveUploadParameter param)
        {
            try
            {
                var pathFile = param.PathFile.Replace(UrlExtension.GetUrl(_context.HttpContext.Request, _env), string.Empty).Trim('/');
                var file = _dbContext.Media.FirstOrDefault(f => f.Path == pathFile
                    && f.RefId == param.RefId
                    && (f.SystemName == param.Path.GetString() || f.SystemName == param.SystemName));

                if (file == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบไฟล์ที่ต้องการลบ";
                    return this.response;
                }

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Media.Remove(file);
                    _dbContext.SaveChanges();
                });

                if (!result.success)
                {
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ลบไฟล์อัพโหลดไม่สำเร็จ";
                    this.response.total = 0;
                    this.response.data = null;
                    this.response.success = false;
                }
                else
                {
                    _dbContext.UploadFileUtilities.RemoveFile(file);
                    this.response.total = 0;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ลบไฟล์อัพโหลดสำเร็จ";
                    this.response.data = new
                    {
                        UID = param.UID
                    };
                    this.response.success = true;
                }

            }
            catch (Exception e)
            {
                _logger.LogError("MediaService.RemoveUpload.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

    }
}