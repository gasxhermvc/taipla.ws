using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using Taipla.Webservice.Models.Parameters.Backend.Um;
using Taipla.Webservice.Models.Responses.Backend.Um;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class UmService : IUmService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;

        private BaseResponse response = new BaseResponse();

        public UmService(TAIPLA_DbContext dbContext, IAuthenticationService<UserInfo> authen, IHttpContextAccessor context, ILoggerFactory logger, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _authen = authen;
            _context = context;
            _logger = logger.CreateLogger("Service");
            _env = env;
        }

        public async Task<BaseResponse> Users(UmUserParameter param)
        {
            try
            {
                var userQuery = _dbContext.User.Where(w =>
                    w.UserId != _authen.User.USER_ID).AsQueryable();

                //=>ผู้ดูแล
                if (this._authen.User.ROLE == RoleEnum.ADMIN.GetString())
                {
                    userQuery = userQuery.Where(w => w.Role != RoleEnum.SUPER_ADMIN.GetString() && w.Role != RoleEnum.ADMIN.GetString());
                }

                //=>เจ้าของร้าน
                if (this._authen.User.ROLE == RoleEnum.OWNER.GetString())
                {
                    userQuery = userQuery.Where(w => w.Role == RoleEnum.STAFF.GetString() && w.ParentId == _authen.User.USER_ID);
                }

                var users = userQuery.ToList();

                List<UmUserResponse> responseData = new List<UmUserResponse>();

                if (users != null && users.Count < 1)
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                    this.response.data = responseData;
                }
                else
                {
                    responseData = (from user in users
                                    let fileNameSplit = user.Avatar?.Split('/') ?? new string[] { }
                                    let fileName = fileNameSplit?.LastOrDefault() ?? string.Empty
                                    let AvatarUrl = !string.IsNullOrEmpty(user.Avatar) ? string.Format("{0}/{1}",
                                _context.HttpContext.Request.GetUrl(_env).Trim(),
                                user.Avatar) : string.Empty
                                    orderby user.CreatedDate descending
                                    select new UmUserResponse
                                    {
                                        USER_ID = user.UserId,
                                        USERNAME = user.Username,
                                        FIRST_NAME = user.FirstName,
                                        LAST_NAME = user.LastName,
                                        PHONE = user.Phone,
                                        EMAIL = user.Email,
                                        ROLE = user.Role,
                                        AVATAR = AvatarUrl,
                                        CLIENT_ID = user.ClientId,
                                        CREATE_DATE = user.CreatedDate,
                                        UPDATE_DATE = user.UpdatedDate,

                                        FULL_NAME = string.Format("{0} {1}",
                                            (!string.IsNullOrEmpty(user.FirstName) ? user.FirstName : string.Empty),
                                            (!string.IsNullOrEmpty(user.LastName) ? user.LastName : string.Empty))
                                    }).ToList();

                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                    this.response.total = responseData.Count;
                    this.response.data = responseData;
                }

                this.response.success = true;
            }
            catch (Exception e)
            {
                _logger.LogError("UmService.Users.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> GetUser(UmUserParameter param)
        {
            try
            {
                if (param.USER_ID == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลผู้ใช้งาน";
                    return this.response;
                }

                var USER_ID = _authen.User.USER_ID;

                if (_authen.User.ROLE == RoleEnum.SUPER_ADMIN.GetString() && param.USER_ID.Value > 0)
                {
                    USER_ID = param.USER_ID.Value;
                }

                if (_authen.User.ROLE == RoleEnum.ADMIN.GetString() && param.USER_ID.Value > 0)
                {
                    USER_ID = param.USER_ID.Value;
                }

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                User user = null;
                if (_authen.User.USER_ID != param.USER_ID)
                {
                    switch (role)
                    {
                        case RoleEnum.OWNER:
                            USER_ID = param.USER_ID.Value;
                            user = _dbContext.User.Where(w =>
                                w.UserId == USER_ID && w.ParentId == _authen.User.USER_ID).FirstOrDefault();
                            break;
                        default:
                            user = _dbContext.User.Where(w =>
                                w.UserId == USER_ID).FirstOrDefault();
                            break;
                    }
                }


                if (user == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลผู้ใช้งาน";
                    return this.response;
                }

                if (this._authen.User.ROLE != RoleEnum.SUPER_ADMIN.GetString())
                {
                    if (user.Role == RoleEnum.ADMIN.GetString() || user.Role == RoleEnum.SUPER_ADMIN.GetString())
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ดึงข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                        return this.response;
                    }
                }

                var fileNameSplit = user.Avatar?.Split('/') ?? new string[] { };
                string fileName = fileNameSplit?.LastOrDefault() ?? string.Empty;
                string AvatarUrl = !string.IsNullOrEmpty(user.Avatar) ? string.Format("{0}/{1}",
                            _context.HttpContext.Request.GetUrl(_env).Trim(),
                            user.Avatar) : string.Empty;

                UmUserResponse responseData = new UmUserResponse()
                {
                    USER_ID = user.UserId,
                    USERNAME = user.Username,
                    FIRST_NAME = user.FirstName,
                    LAST_NAME = user.LastName,
                    PHONE = user.Phone,
                    EMAIL = user.Email,
                    ROLE = user.Role,
                    AVATAR = AvatarUrl,
                    CLIENT_ID = user.ClientId,
                    CREATE_DATE = user.CreatedDate,
                    UPDATE_DATE = user.UpdatedDate,

                    FULL_NAME = string.Format("{0} {1}",
                                            (!string.IsNullOrEmpty(user.FirstName) ? user.FirstName : string.Empty),
                                            (!string.IsNullOrEmpty(user.LastName) ? user.LastName : string.Empty)),

                    UPLOAD_FILES = (!string.IsNullOrEmpty(AvatarUrl) ? new List<NzUploadFile> {
                        new NzUploadFile
                        {
                            name = fileName,
                            url = AvatarUrl,
                        } } : new List<NzUploadFile>())
                };

                this.response.success = true;
                this.response.statusCode = (int)HttpStatusCode.OK;
                this.response.message = "ดึงข้อมูลสำเร็จ";
                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("UmService.User.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Created(UmCreatedParameter param)
        {
            try
            {
                if (this._authen.User.ROLE == RoleEnum.STAFF.GetString() || this._authen.User.ROLE == RoleEnum.CLIENT.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "เพิ่มข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                    return this.response;
                }

                //=>Admin
                if (this._authen.User.ROLE == RoleEnum.ADMIN.GetString() && param.ROLE == RoleEnum.ADMIN.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "เพิ่มข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                    return this.response;
                }

                //=>Owner
                if (this._authen.User.ROLE == RoleEnum.OWNER.GetString() && param.ROLE != RoleEnum.STAFF.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "เพิ่มข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                    return this.response;
                }

                var _user = _dbContext.User.FirstOrDefault(f =>
                    f.Username == param.USERNAME ||
                    f.Email == param.EMAIL);

                if (_user != null)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "มีรายการในระบบแล้ว";

                    return this.response;
                }


                var now = DateTime.Now;

                User user = new User
                {
                    Username = param.USERNAME,
                    Password = HashExtension.Sha256(param.PASSWORD),
                    ClientId = param.CLIENT_ID,
                    FirstName = param.FIRST_NAME,
                    LastName = param.LAST_NAME,
                    Email = param.EMAIL,
                    Phone = param.PHONE,
                    Role = ValidateExtension.isRoleValid(param.ROLE) ? param.ROLE : RoleEnum.CLIENT.GetString(),
                    CreatedDate = now,
                    UpdatedDate = now
                };

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.OWNER:
                        user.ParentId = _authen.User.USER_ID;
                        break;
                    default:
                        break;
                }

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.User.Add(user);
                    _dbContext.SaveChanges();

                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.UM.GetString(),
                            fileName,
                            user.UserId.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.UPLOAD);

                        user.Avatar = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.User.Update(user);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.Created;
                    this.response.message = "บันทึกข้อมูลสำเร็จ";
                    this.response.data = new
                    {
                        USER_ID = user.UserId
                    };
                }
                else
                {
                    _logger.LogError("UmService.Created.Transaction.Exception: {0}", result.exception.ToString());
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("UmService.Created.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }
        public async Task<BaseResponse> Updated(UmUpdatedParameter param)
        {
            try
            {

                if (this._authen.User.ROLE == RoleEnum.STAFF.GetString() || this._authen.User.ROLE == RoleEnum.CLIENT.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "แก้ไขข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                    return this.response;
                }

                //=>Admin
                if (this._authen.User.ROLE == RoleEnum.ADMIN.GetString() && param.ROLE == RoleEnum.ADMIN.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "แก้ไขข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                    return this.response;
                }

                //=>Owner
                if (this._authen.User.ROLE == RoleEnum.OWNER.GetString() && param.ROLE != RoleEnum.STAFF.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "แก้ไขข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                    return this.response;
                }

                var user = _dbContext.User.FirstOrDefault(f =>
                    f.UserId == param.USER_ID &&
                    f.Username == param.USERNAME);

                if (user == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลผู้ใช้งาน";

                    return this.response;
                }

                var now = DateTime.Now;

                user.FirstName = param.FIRST_NAME;
                user.LastName = param.LAST_NAME;
                user.Email = param.EMAIL;
                user.Phone = param.PHONE;
                user.Role = ValidateExtension.isRoleValid(param.ROLE) ? param.ROLE : RoleEnum.CLIENT.GetString();
                user.UpdatedDate = now;

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.User.Update(user);
                    _dbContext.SaveChanges();
                    if (param.UPLOAD != null)
                    {
                        string fileName = param.UPLOAD.FileName;

                        upload = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.UM.GetString(),
                            user.UserId.ToString());

                        if (upload == null)
                        {
                            upload = _dbContext.UploadFileUtilities.CreateMedia(
                                 UploadEnum.UM.GetString(),
                                 fileName,
                                 user.UserId.ToString(),
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

                        user.Avatar = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.User.Update(user);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "แก้ไขข้อมูลสำเร็จ";
                    this.response.data = new
                    {
                        USER_ID = user.UserId
                    };
                }
                else
                {
                    _logger.LogError("UmService.Updated.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("UmService.Updated.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }
        public async Task<BaseResponse> Deleted(UmDeletedParameter param)
        {
            try
            {


                var user = _dbContext.User.FirstOrDefault(f =>
                    f.UserId == param.USER_ID);

                if (user == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลผู้ใช้งาน";

                    return this.response;
                }

                if (this._authen.User.ROLE == RoleEnum.STAFF.GetString() || this._authen.User.ROLE == RoleEnum.CLIENT.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "ลบข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                    return this.response;
                }

                //=>Admin
                if (this._authen.User.ROLE == RoleEnum.ADMIN.GetString() && user.Role == RoleEnum.ADMIN.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "ลบข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                    return this.response;
                }

                //=>Owner
                if (this._authen.User.ROLE == RoleEnum.OWNER.GetString() && user.Role != RoleEnum.STAFF.GetString())
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "ลบข้อมูลผู้ใช้งานไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึง";
                    return this.response;
                }

                if (this._authen.User.ROLE != RoleEnum.SUPER_ADMIN.GetString())
                {
                    if (user.Role == RoleEnum.ADMIN.GetString())
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่มีสิทธิ์ลบข้อมูล";

                        return this.response;
                    }
                }


                var media = _dbContext.UploadFileUtilities.DeleteMedia(user.Avatar);


                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.User.Remove(user);
                    if (media != null)
                    {
                        _dbContext.Media.Remove(media);
                        //=>ต้องการลบไฟล์ด้วย
                        //_dbContext.UploadFileUtilities.RemoveFile(media);
                        //_dbContext.UploadFileUtilities.RemoveFolder(user.Avatar);
                    }
                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    _dbContext.UploadFileUtilities.RemoveFolder(user.Avatar);
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ลบข้อมูลสำเร็จ";
                    this.response.data = new
                    {
                        USER_ID = user.UserId
                    };
                }
                else
                {
                    _logger.LogError("UmService.Deleted.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("UmService.Deleted.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> ChangePassword(UmChangePasswordParameter param)
        {
            try
            {

                var user = _dbContext.User.FirstOrDefault(f =>
                    f.UserId == param.USER_ID);

                if (user == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ไม่พบข้อมูลผู้ใช้งาน";

                    return this.response;
                }

                if (_authen.User.ROLE != RoleEnum.ADMIN.GetString())
                {
                    if (user.UserId != _authen.User.USER_ID)
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่มีสิทธิ์เปลี่ยนข้อมูล";

                        return this.response;
                    }

                    if (user.Password != HashExtension.Sha256(param.PASSWORD_OLD))
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Conflict;
                        this.response.message = "ไม่สามารถบันทึกรหัสผ่านใหม่ เนื่องจากรหัสผ่านเดิมไม่ถูกไม่ต้อง";

                        return this.response;
                    }

                }

                user.Password = HashExtension.Sha256(param.PASSWORD_NEW);
                user.UpdatedDate = DateTime.Now;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.User.Update(user);
                    _dbContext.SaveChanges();
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "เปลี่ยนรหัสผ่านสำเร็จ";
                }
                else
                {
                    _logger.LogError("UmService.ChangePassword.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = "กรุณาลองใหม่อีกครั้ง";
                }
            }
            catch (Exception e)
            {
                _logger.LogError("UmService.ChangePassword.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
            }

            return await Task.Run(() => this.response);
        }
    }
}
