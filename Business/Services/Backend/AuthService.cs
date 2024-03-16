using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Auth;
using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class AuthService : IAuthService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<UserInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IDetectionService _detectionService;

        private BaseResponse response = new BaseResponse();

        public AuthService(TAIPLA_DbContext dbContext,
            IAuthenticationService<UserInfo> authen,
            IHttpContextAccessor context,
            ILoggerFactory logger,
            IDetectionService detectionService)
        {
            _dbContext = dbContext;
            _authen = authen;
            _context = context;
            _logger = logger.CreateLogger("Service");
            _detectionService = detectionService;
        }

        public async Task<BaseResponse> Login(LoginParameter param)
        {
            try
            {

                var user = _dbContext.User.FirstOrDefault(f => f.Username == param.USERNAME && f.Password == HashExtension.Sha256(param.PASSWORD));

                if (user != null)
                {
                    if(user.Role == RoleEnum.CLIENT.GetString())
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Forbidden;
                        this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงระบบ";
                        this.response.data = null;

                        return this.response;
                    }

                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Username)
                    };

                    var deviceName = _detectionService.GetDevicePlatform();

                    var deviceId = _detectionService.GetDeviceId(_context, _authen, user.ClientId);

                    var now = DateTime.Now;
                    var token = _authen.GenerateToken(claims, param?.REMEMBER_ME ?? false);

                    //UserDevice deviceAccess = _dbContext.UserDevice.FirstOrDefault(f => f.DeviceId == deviceId);
                    UserDevice deviceAccess = _dbContext.UserUtilities.GetDevice(deviceId);

                    var result = _dbContext.Utility.CreateTransaction(() =>
                    {
                        if (deviceAccess == null)
                        {
                            deviceAccess = new UserDevice
                            {
                                ClientId = user.ClientId,
                                DeviceId = deviceId,
                                Token = token["token"],
                                Expired = token["expired"],
                                DeviceType = deviceName,
                                CreatedDate = now,
                                UpdatedDate = now
                            };
                            _dbContext.UserDevice.Add(deviceAccess);
                        }
                        else
                        {
                            deviceAccess.Token = token["token"];
                            deviceAccess.Expired = token["expired"];
                            deviceAccess.UpdatedDate = now;
                            _dbContext.UserDevice.Update(deviceAccess);
                        }

                        _dbContext.SaveChanges();
                    });

                    if (!result.success)
                    {
                        _logger.LogError("Backend.AuthService.Login.Transaction.Exception: {0}", result.exception.ToString());
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.BadRequest;
                        this.response.message = "เข้าสู่ระบบไม่สำเร็จ กรุณาลองใหม่อีกครั้ง";
                        return this.response;
                    }

                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "เข้าสู่ระบบสำเร็จ";
                    this.response.data = new
                    {
                        token = deviceAccess.Token,
                        expired = deviceAccess.Expired,
                        client_id = deviceAccess.ClientId
                    };
                }
                else
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Unauthorized;
                    this.response.message = "Username หรือ Password ไม่ถูกต้อง";
                    this.response.data = null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Backend.AuthService.Login.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Logout(string CLIENT_ID)
        {
            try
            {
                var deviceName = _detectionService.GetDevicePlatform();

                var deviceId = _detectionService.GetDeviceId(_context, _authen, CLIENT_ID);

                UserDevice deviceAccess = _dbContext.UserDevice.FirstOrDefault(f => f.DeviceId == deviceId);

                if (deviceAccess != null)
                {
                    var result = _dbContext.Utility.CreateTransaction(() =>
                    {
                        deviceAccess.Token = string.Empty;
                        deviceAccess.Expired = DateTime.Now.AddYears(-1);
                        _dbContext.UserDevice.Update(deviceAccess);
                        _dbContext.SaveChanges();
                    });

                    if (!result.success)
                    {
                        _logger.LogError("Backend.AuthService.Logout.Transaction.Exception: {0}", result.exception.ToString());

                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.BadRequest;
                        this.response.message = "ออกจากระบบไม่สำเร็จ กรุณาลองใหม่อีกครั้ง";
                        return this.response;
                    }

                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ออกจากระบบสำเร็จ";
                    this.response.data = null;
                }
                else
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = "ออกจากระบบไม่สำเร็จ, เนื่องจากไม่พบข้อมูลการเชื่อมต่ออุปกรณ์";
                    this.response.data = null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Backend.AuthService.Logout.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> UserInfo()
        {
            try
            {
                if (_authen.IsAuthenticated)
                {
                    if (this._authen.User != null)
                    {
                        if (this._authen.User.ROLE == RoleEnum.CLIENT.GetString())
                        {
                            this.response.success = false;
                            this.response.statusCode = (int)HttpStatusCode.Forbidden;
                            this.response.message = "ไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงระบบ";
                            this.response.data = null;

                            return this.response;
                        }

                        this.response.success = true;
                        this.response.statusCode = (int)HttpStatusCode.OK;
                        this.response.message = "ดึงข้อมูลสำเร็จ";
                        this.response.data = this._authen.User;
                        return this.response;
                    }
                }
                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.Unauthorized;
                this.response.data = null;
                this.response.message = "ยังไม่ได้ยืนยันตัวตน";

            }
            catch (Exception e)
            {
                _logger.LogError("Backend.AuthService.UserInfo.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }
    }
}
