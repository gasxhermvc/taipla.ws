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
using Taipla.Webservice.Models.Parameters.Backend.Promotion;
using Taipla.Webservice.Models.Responses.Backend.Promotion;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class PromotionService : IPromotionService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        private BaseResponse response = new BaseResponse();

        public PromotionService(TAIPLA_DbContext dbContext,
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

        public async Task<BaseResponse> Promotions(PromotionPromotionsParameter param)
        {
            try
            {
                if (param.RES_ID != null && param.RES_ID < 1)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
                    return this.response;
                }

                if (param.PROMOTION_TYPE != null && !ValidateExtension.isPromotionTypeValid((int)param.PROMOTION_TYPE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ประเภทของโปรโมชันไม่ถูกต้อง";
                    return this.response;
                }

                if (!ValidateExtension.isDateTimeBetweenIsValid(param.START_DATE, param.END_DATE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, รูปแบบวันที่เริ่มต้น-สิ้นสุดไม่ถูกต้อง";
                    return this.response;
                }

                int USER_ID;
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        if (param.OWNER_ID == null || param.OWNER_ID < 1)
                        {
                            USER_ID = -1;
                        }
                        else
                        {
                            USER_ID = param.OWNER_ID.Value;
                        }
                        break;
                    case RoleEnum.OWNER:
                        if (param.RES_ID == null || param.RES_ID < 1)
                        {
                            this.response.success = false;
                            this.response.statusCode = (int)HttpStatusCode.Forbidden;
                            this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                            this.response.data = null;
                            return this.response;
                        }

                        USER_ID = _authen.User.USER_ID;
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = null;
                        return this.response;
                }
                Restaurant restaurant = null;
                IQueryable<Restaurant> resQuery = null;

                if (param.RES_ID != null && param.RES_ID > 0 || param.OWNER_ID != null && param.OWNER_ID > 0 || _authen.User.ROLE == RoleEnum.OWNER.GetString())
                {
                    resQuery = _dbContext.Restaurant.AsQueryable();

                    if (param.RES_ID != null && param.RES_ID > 0)
                    {
                        resQuery = _dbContext.Restaurant.Where(w => w.ResId == param.RES_ID);
                    }

                    if (USER_ID > 0)
                    {
                        resQuery = _dbContext.Restaurant.Where(w => w.OwnerId == USER_ID);
                    }

                    restaurant = resQuery.FirstOrDefault();

                    if (restaurant == null)
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.NotFound;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่พบข้อมูลร้านอาหาร";
                        return this.response;
                    }
                }

                var proQuery = _dbContext.Promotion.AsQueryable();

                if (restaurant != null)
                {
                    proQuery = proQuery.Where(w => w.ResId == restaurant.ResId);
                }

                if (param.RES_ID != null && param.RES_ID > 0)
                {
                    proQuery = proQuery.Where(w => w.ResId == param.RES_ID);
                }

                if (param.PROMOTION_TYPE != null && ValidateExtension.isPromotionTypeValid((int)param.PROMOTION_TYPE))
                {
                    proQuery = proQuery.Where(w => w.Flag == param.PROMOTION_TYPE);
                }

                if (param.PROMOTION_ID != null && param.PROMOTION_ID > 0)
                {
                    proQuery = proQuery.Where(w => w.Id == param.PROMOTION_ID);
                }

                if (param.START_DATE != null && param.END_DATE != null && ValidateExtension.isDateTimeBetweenIsValid(param.START_DATE, param.END_DATE))
                {
                    proQuery = proQuery.Where(w => w.StartDate >= param.START_DATE && w.EndDate <= param.END_DATE);
                }

                var promotions = proQuery.ToList();

                List<PromotionResponse> responseData = new List<PromotionResponse>();

                if (promotions != null && promotions.Count < 1)
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูลอาหาร";
                    this.response.data = responseData;
                }
                else
                {
                    responseData = (from promotion in promotions
                                    let fileNameSplit = promotion.Thumbnail?.Split('/') ?? new string[] { }
                                    let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                    let ThumbnailUrl = !string.IsNullOrEmpty(promotion.Thumbnail) ? string.Format("{0}/{1}",
                                    _context.HttpContext.Request.GetUrl(_env).Trim(),
                                    promotion.Thumbnail) : string.Empty
                                    let flag = (PromotionEnum)promotion.Flag
                                    orderby promotion.CreatedDate descending
                                    select new PromotionResponse
                                    {
                                        PROMOTION_ID = promotion.Id,
                                        RES_ID = promotion.ResId,
                                        NAME = promotion.Name?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        DESCRIPTION = promotion.Description?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                                        PROMOTION_TYPE = ((int)flag).ToString(),
                                        PROMOTION_TYPE_DESC = flag.GetString(),
                                        THUMBNAIL = ThumbnailUrl,
                                        VIEWER = promotion.Viewer,
                                        START_DATE = promotion.StartDate != null ? promotion.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                                        END_DATE = promotion.EndDate != null ? promotion.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                                        CREATE_DATE = promotion.CreatedDate,
                                        UPDATE_DATE = promotion.UpdatedDate
                                    }).ToList();

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลโปรโมชันสำเร็จ";
                    this.response.total = responseData.Count;
                    this.response.data = responseData;
                }
                this.response.success = true;
            }
            catch (Exception e)
            {
                _logger.LogError("PromotionService.Promotions.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> GetPromotion(PromotionPromotionsParameter param)
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

                if (!ValidateExtension.isPromotionTypeValid((int)param.PROMOTION_TYPE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ประเภทของโปรโมชันไม่ถูกต้อง";
                    return this.response;
                }

                if (!ValidateExtension.isDateTimeBetweenIsValid(param.START_DATE, param.END_DATE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, รูปแบบวันที่เริ่มต้น-สิ้นสุดไม่ถูกต้อง";
                    return this.response;
                }

                int USER_ID;
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        if (param.OWNER_ID == null || param.OWNER_ID < 1)
                        {
                            USER_ID = -1;
                        }
                        else
                        {
                            USER_ID = param.OWNER_ID.Value;
                        }
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

                var proQuery = _dbContext.Promotion.Where(w => w.Id == param.PROMOTION_ID && w.ResId == restaurant.ResId).AsQueryable();

                var promotion = proQuery.FirstOrDefault();

                if (promotion == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, ไม่พบข้อมูลโปรโมชัน";
                    return this.response;
                }

                var fileNameSplit = promotion.Thumbnail?.Split('/') ?? new string[] { };
                string fileName = fileNameSplit?.LastOrDefault() ?? string.Empty;
                string ThumbnailUrl = !string.IsNullOrEmpty(promotion.Thumbnail) ? string.Format("{0}/{1}",
                            _context.HttpContext.Request.GetUrl(_env).Trim(),
                            promotion.Thumbnail) : string.Empty;

                PromotionEnum flag = (PromotionEnum)promotion.Flag;

                var responData = new PromotionResponse
                {
                    PROMOTION_ID = promotion.Id,
                    RES_ID = promotion.ResId,
                    NAME = promotion.Name?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    DESCRIPTION = promotion.Description?.Trim().DecodeSpacialCharacters() ?? string.Empty,
                    PROMOTION_TYPE = ((int)flag).ToString(),
                    PROMOTION_TYPE_DESC = flag.GetString(),
                    THUMBNAIL = ThumbnailUrl,
                    VIEWER = promotion.Viewer,
                    START_DATE = promotion.StartDate != null ? promotion.StartDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                    END_DATE = promotion.EndDate != null ? promotion.EndDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                    CREATE_DATE = promotion.CreatedDate,
                    UPDATE_DATE = promotion.UpdatedDate,

                    UPLOAD_FILES = (!string.IsNullOrEmpty(ThumbnailUrl) ? new List<NzUploadFile> {
                        new NzUploadFile
                        {
                            name = fileName,
                            url = ThumbnailUrl,
                        } } : new List<NzUploadFile>())
                };

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = "ดึงข้อมูลโปรโมชันสำเร็จ";
                this.response.data = responData;
            }
            catch (Exception e)
            {
                _logger.LogError("PromotionService.GetPromotion.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Created(PromotionCreatedParameter param)
        {
            try
            {
                if (param.RES_ID < 1)
                {

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
                    return this.response;
                }

                if (!ValidateExtension.isPromotionTypeValid((int)param.PROMOTION_TYPE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ประเภทของโปรโมชันไม่ถูกต้อง";
                    return this.response;
                }

                if (!ValidateExtension.isDateTimeBetweenIsValid(param.START_DATE, param.END_DATE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, รูปแบบวันที่เริ่มต้น-สิ้นสุดไม่ถูกต้อง";
                    return this.response;
                }

                int USER_ID;
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        if (param.OWNER_ID == null || param.OWNER_ID < 1)
                        {
                            USER_ID = -1;
                        }
                        else
                        {
                            USER_ID = param.OWNER_ID.Value;
                        }
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

                var proQuery = _dbContext.Promotion.Where(w => w.ResId == restaurant.ResId).AsQueryable();

                switch (param.PROMOTION_TYPE)
                {

                    case PromotionEnum.HIDE:
                        break;
                    case PromotionEnum.USE_ESTIMATE:
                        if (param.START_DATE != null && param.END_DATE != null)
                        {
                            param.END_DATE = DateTime.Parse(param.END_DATE.Value.ToString("yyyy-MM-dd 23:59:59"));
                        }
                        else
                        {
                            param.START_DATE = DateTime.Now.Date;
                            param.END_DATE = DateTime.Parse(DateTime.Now.Date.ToString("yyyy-MM-dd 23:59:59"));
                        }
                        break;
                    case PromotionEnum.FREEZE:
                        //proQuery = proQuery.Where(w => w.Flag == (int) w.StartDate >= param.START_DATE && w.EndDate <= param.END_DATE).AsQueryable();
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.BadRequest;
                        this.response.message = "ไม่สำเร็จ, ประเภทของโปรโมชันไม่ถูกต้อง";
                        return this.response;
                }

                var now = DateTime.Now;

                Promotion promotion = new Promotion
                {
                    ResId = restaurant.ResId,
                    Name = param.NAME?.Trim().EncodeSpacialCharacters() ?? string.Empty,
                    Description = param.DESCRIPTION?.Trim().EncodeSpacialCharacters() ?? string.Empty,
                    Flag = (int)param.PROMOTION_TYPE,
                    StartDate = param.START_DATE,
                    EndDate = param.END_DATE,
                    CreatedDate = now,
                    UpdatedDate = now
                };

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Promotion.Add(promotion);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.PROMOTION.GetString(),
                            fileName,
                            promotion.Id.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.UPLOAD);

                        promotion.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.Promotion.Update(promotion);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.Created;
                    this.response.message = "บันทึกข้อมูลโปรโมชันสำเร็จ";
                    this.response.data = new
                    {
                        PROMOTION_ID = promotion.Id,
                        RES_ID = promotion.ResId,
                        PROMOTION_TYPE = promotion.Flag
                    };
                }
                else
                {
                    _logger.LogError("PromotionService.Created.Transaction.Exception: {0}", result.exception.ToString());
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("PromotionService.Created.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Updated(PromotionUpdatedParameter param)
        {
            try
            {
                if (param.RES_ID < 1)
                {

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
                    return this.response;
                }

                if (!ValidateExtension.isPromotionTypeValid((int)param.PROMOTION_TYPE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ประเภทของโปรโมชันไม่ถูกต้อง";
                    return this.response;
                }

                if (!ValidateExtension.isDateTimeBetweenIsValid(param.START_DATE, param.END_DATE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, รูปแบบวันที่เริ่มต้น-สิ้นสุดไม่ถูกต้อง";
                    return this.response;
                }

                int USER_ID;
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        if (param.OWNER_ID == null || param.OWNER_ID < 1)
                        {
                            USER_ID = -1;
                        }
                        else
                        {
                            USER_ID = param.OWNER_ID.Value;
                        }
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

                var proQuery = _dbContext.Promotion.Where(w =>
                w.Id == param.PROMOTION_ID && w.ResId == restaurant.ResId).AsQueryable();

                switch (param.PROMOTION_TYPE)
                {

                    case PromotionEnum.HIDE:
                        break;
                    case PromotionEnum.USE_ESTIMATE:
                        if (param.START_DATE != null && param.END_DATE != null)
                        {
                            param.END_DATE = DateTime.Parse(param.END_DATE.Value.ToString("yyyy-MM-dd 23:59:59"));
                        }
                        else
                        {
                            param.START_DATE = DateTime.Now.Date;
                            param.END_DATE = DateTime.Parse(DateTime.Now.Date.ToString("yyyy-MM-dd 23:59:59"));
                        }
                        break;
                    case PromotionEnum.FREEZE:
                        //proQuery = proQuery.Where(w => w.Flag == (int) w.StartDate >= param.START_DATE && w.EndDate <= param.END_DATE).AsQueryable();
                        break;
                    default:
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.BadRequest;
                        this.response.message = "ไม่สำเร็จ, ประเภทของโปรโมชันไม่ถูกต้อง";
                        return this.response;
                }

                var promotion = proQuery.FirstOrDefault();

                if (promotion == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, ไม่พบข้อมูลโปรโมชัน";
                    return this.response;
                }

                var now = DateTime.Now;
                promotion.Name = param.NAME?.Trim().EncodeSpacialCharacters() ?? string.Empty;
                promotion.Description = param.DESCRIPTION?.Trim().EncodeSpacialCharacters() ?? string.Empty;
                promotion.Flag = (int)param.PROMOTION_TYPE;
                promotion.StartDate = param.START_DATE;
                promotion.EndDate = param.END_DATE;
                promotion.UpdatedDate = now;

                if (param.PROMOTION_TYPE != PromotionEnum.USE_ESTIMATE)
                {
                    promotion.StartDate = (DateTime?)null;
                    promotion.EndDate = (DateTime?)null;
                }

                Media upload = null;
                Media oldUpload = _dbContext.UploadFileUtilities.GetMedia(
                    UploadEnum.PROMOTION.GetString()
                    , promotion.Id.ToString()).CopyTo<Media, Media>();

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Promotion.Update(promotion);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.PROMOTION.GetString(),
                            promotion.Id.ToString());

                        if (upload == null)
                        {
                            upload = _dbContext.UploadFileUtilities.CreateMedia(
                                 UploadEnum.PROMOTION.GetString(),
                                 fileName,
                                 promotion.Id.ToString(),
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

                        promotion.Thumbnail = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.Promotion.Update(promotion);
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
                    this.response.message = "แก้ไขข้อมูลโปรโมชันสำเร็จ";
                    this.response.data = new
                    {
                        PROMOTION_ID = promotion.Id,
                        RES_ID = promotion.ResId,
                        PROMOTION_TYPE = promotion.Flag
                    };
                }
                else
                {
                    _logger.LogError("PromotionService.Updated.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("PromotionService.Updated.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Deleted(PromotionDeletedParameter param)
        {
            try
            {
                if (param.RES_ID < 1)
                {

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ต้องการรหัสร้านอาหารด้วย";
                    return this.response;
                }

                if (!ValidateExtension.isPromotionTypeValid((int)param.PROMOTION_TYPE))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ประเภทของโปรโมชันไม่ถูกต้อง";
                    return this.response;
                }

                int USER_ID;
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST_RESTAURANT:
                        if (param.OWNER_ID == null || param.OWNER_ID < 1)
                        {
                            USER_ID = -1;
                        }
                        else
                        {
                            USER_ID = param.OWNER_ID.Value;
                        }
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

                var proQuery = _dbContext.Promotion.Where(w =>
                    w.Id == param.PROMOTION_ID && w.ResId == restaurant.ResId).AsQueryable();

                var promotion = proQuery.FirstOrDefault();

                if (promotion == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่สำเร็จ, ไม่พบข้อมูลโปรโมชัน";
                    return this.response;
                }

                if (promotion.Flag != (int)param.PROMOTION_TYPE)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "ไม่สำเร็จ, ประเภทของโปรโมชันไม่ถูกต้อง";
                    return this.response;
                }

                var media = _dbContext.UploadFileUtilities.DeleteMedia(promotion.Thumbnail);

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.Promotion.Remove(promotion);
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
                    _dbContext.UploadFileUtilities.RemoveFolder(promotion.Thumbnail);
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ลบข้อมูลโปรโมชันสำเร็จ";
                    this.response.data = new
                    {
                        PROMOTION_ID = promotion.Id,
                        RES_ID = promotion.ResId
                    };
                }
                else
                {
                    _logger.LogError("PromotionService.Deleted.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("PromotionService.Deleted.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }
    }
}
