using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Auth;
using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;

namespace Taipla.Webservice.Business.Services.Frontend
{
    public class AuthService : IAuthService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<ClientInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IDetectionService _detectionService;
        private readonly MessageFactory _message;

        private BaseResponse response = new BaseResponse();

        private const string ROOT_MESSAGE = "Frontend:AuthController";

        public AuthService(TAIPLA_DbContext dbContext,
            IAuthenticationService<ClientInfo> authen,
            IHttpContextAccessor context,
            ILoggerFactory logger,
            IDetectionService detectionService,
            MessageFactory messageFactory)
        {
            _dbContext = dbContext;
            _authen = authen;
            _context = context;
            _logger = logger.CreateLogger("Service");
            _detectionService = detectionService;
            _message = messageFactory;
        }
        public async Task<BaseResponse> Login(LoginParameter param)
        {
            try
            {

                var user = _dbContext.User.FirstOrDefault(f => f.Username == param.USERNAME && f.Password == HashExtension.Sha256(param.PASSWORD));

                if (user != null)
                {
                    List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Username)
                    };

                    var now = DateTime.Now;
                    var token = _authen.GenerateToken(claims, param?.REMEMBER_ME ?? false);

                    var deviceName = _detectionService.GetDevicePlatform();

                    var deviceId = string.Empty;

                    if (!string.IsNullOrEmpty(param.DEVICE_NAME))
                    {
                        deviceId = param.DEVICE_NAME;
                    }
                    else
                    {
                        deviceId = _detectionService.GetDeviceId(_context, _authen, user.ClientId);
                    }

                    UserDevice deviceAccess = _dbContext.UserDevice.FirstOrDefault(f => f.DeviceId == deviceId);

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
                        _logger.LogError("Frontend.AuthService.Login.Transaction.Exception: {0}", result.exception.ToString());
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.BadRequest;
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "Login:400_BAD_REQUEST");
                        return this.response;
                    }

                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Login:200_SUCCESS");
                    this.response.data = new
                    {
                        token = deviceAccess.Token,
                        expired = deviceAccess.Expired,
                        clientId = deviceAccess.ClientId
                    };
                }
                else
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Unauthorized;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Login:401_UN_AUTHORIZE");
                    this.response.data = null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.AuthService.Login.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Login:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Logout(LogoutParameter param)
        {
            try
            {
                var deviceName = _detectionService.GetDevicePlatform();

                var deviceId = param.DEVICE_ID;

                UserDevice deviceAccess = _dbContext.UserDevice.FirstOrDefault(f => f.DeviceId == deviceId);

                if (deviceAccess != null)
                {
                    if (deviceAccess.ClientId != param.CLIENT_ID)
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.Unauthorized;
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "Logout:401_UN_AUTHORIZE_CLIENT_ID");
                        this.response.data = null;

                        return this.response;
                    }

                    if (string.IsNullOrEmpty(deviceAccess.Token))
                    {
                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.NoContent;
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "Logout:204_TOKEN_IS_EMPTY");
                        this.response.data = null;

                        return this.response;
                    }

                    var result = _dbContext.Utility.CreateTransaction(() =>
                    {
                        deviceAccess.Token = string.Empty;
                        deviceAccess.Expired = DateTime.Now.AddYears(-1);
                        _dbContext.UserDevice.Update(deviceAccess);
                        _dbContext.SaveChanges();
                    });

                    if (!result.success)
                    {
                        _logger.LogError("Frontend.AuthService.Logout.Transaction.Exception: {0}", result.exception.ToString());

                        this.response.success = false;
                        this.response.statusCode = (int)HttpStatusCode.BadRequest;
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "Logout:400_BAD_REQUEST");
                        return this.response;
                    }

                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Logout:200_SUCCESS");
                    this.response.data = null;
                }
                else
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Unauthorized;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Logout:401_UN_AUTHORIZE_DEVICE_ID");
                    this.response.data = null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.AuthService.Logout.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Logout:500_INTERNAL_SERVER_ERROR");
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
                        this.response.success = true;
                        this.response.statusCode = (int)HttpStatusCode.OK;
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "UserInfo:200_SUCCESS");
                        this.response.data = this._authen.User;
                        return this.response;
                    }
                }
                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.Unauthorized;
                this.response.data = null;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "UserInfo:401_UN_AUTHORIZE");

            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.AuthService.UserInfo.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "UserInfo:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

    }
}
