using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Account;

namespace Taipla.Webservice.Business.Services.Frontend
{
    public class AccountService : IAccountService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IAuthenticationService<ClientInfo> _authen;
        private readonly IHttpContextAccessor _context;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;
        private readonly MessageFactory _message;

        private BaseResponse response = new BaseResponse();

        private const string ROOT_MESSAGE = "Frontend:AccountController";

        public AccountService(TAIPLA_DbContext dbContext
            , IAuthenticationService<ClientInfo> authen
            , IHttpContextAccessor context
            , ILoggerFactory logger
            , IWebHostEnvironment env
            , MessageFactory messageFactory)
        {
            _dbContext = dbContext;
            _authen = authen;
            _context = context;
            _logger = logger.CreateLogger("Service");
            _env = env;
            _message = messageFactory;
        }

        public async Task<BaseResponse> Register(RegisterParameter param)
        {
            try
            {
                var _user = _dbContext.User.FirstOrDefault(f =>
                    f.Username == param.USERNAME ||
                    f.Email == param.EMAIL);

                if (_user != null) //=>Not found
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;

                    if (_user.Username == param.USERNAME)
                    {
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "Register:200_USERNAME_DUPLICATE");
                    }
                    else if (_user.Email == param.EMAIL)
                    {
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "Register:200_EMAIL_DUPLICATE");
                    }

                    return this.response;
                }


                var now = DateTime.Now;

                User user = new User
                {
                    Username = param.USERNAME,
                    Password = HashExtension.Sha256(param.PASSWORD),
                    //ClientId = param.DEVICE_ID,
                    ClientId = Guid.NewGuid().ToString(),
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    Email = param.EMAIL,
                    Phone = ValidateExtension.ReplaceWithPhoneNumberThai(param.PHONE_NUMBER),
                    Role = RoleEnum.CLIENT.GetString(),
                    CreatedDate = now,
                    UpdatedDate = now
                };

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.User.Add(user);
                    _dbContext.SaveChanges();

                    if (param.AVATAR_IMAGE != null)
                    {
                        string fileName = param.AVATAR_IMAGE.FileName;

                        upload = _dbContext.UploadFileUtilities.CreateMedia(
                            UploadEnum.CLIENT.GetString(),
                            fileName,
                            user.UserId.ToString(),
                            now);

                        _dbContext.Media.Add(upload);
                        _dbContext.SaveChanges();

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.AVATAR_IMAGE);

                        user.Avatar = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.User.Update(user);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.Created;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Register:201_CREATED");
                    this.response.data = new
                    {
                        USER_ID = user.UserId
                    };
                }
                else
                {
                    _logger.LogError("Frontend.AccountService.Register.Transaction.Exception: {0}", result.exception.ToString());
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "Register:400_BAD_REQUEST");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.AccountService.Register.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Register:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Profile()
        {
            try
            {
                if (_authen.IsAuthenticated)
                {
                    if (this._authen.User != null)
                    {
                        this.response.success = true;
                        this.response.statusCode = (int)HttpStatusCode.OK;
                        this.response.message = _message.GetMessage(ROOT_MESSAGE, "Profile:200_SUCCESS");
                        this.response.data = this._authen.User;
                        return this.response;
                    }
                }

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.Unauthorized;
                this.response.data = null;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Profile:401_UN_AUTHORIZE");

            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.AuthService.Profile.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "Profile:500_INTERNAL_SERVER_ERROR");
                this.response.data = null;
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> UpdateProfile(AccountProfileParameter param)
        {
            try
            {
                var user = _dbContext.User.FirstOrDefault(f =>
                    f.UserId == _authen.User.USER_ID &&
                    f.Username == _authen.User.USERNAME);

                if (user == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "UpdateProfile:404_USER_NOT_FOUND");

                    return this.response;
                }

                var now = DateTime.Now;

                user.FirstName = param.FIRST_NAME;
                user.LastName = param.LAST_NAME;
                user.Email = param.EMAIL;
                user.Phone = param.PHONE_NUMBER;
                user.UpdatedDate = now;

                Media upload = null;

                var result = _dbContext.Utility.CreateTransaction(() =>
                {
                    _dbContext.User.Update(user);
                    _dbContext.SaveChanges();
                    if (param.AVATAR_IMAGE != null)
                    {
                        string fileName = param.AVATAR_IMAGE.FileName;

                        upload = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.CLIENT.GetString(),
                            user.UserId.ToString());

                        if (upload == null)
                        {
                            upload = _dbContext.UploadFileUtilities.CreateMedia(
                                 UploadEnum.CLIENT.GetString(),
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

                        _dbContext.UploadFileUtilities.SaveAs(upload, param.AVATAR_IMAGE);

                        user.Avatar = _dbContext.UploadFileUtilities.GetPhysicalPath(upload);
                        _dbContext.User.Update(user);
                        _dbContext.SaveChanges();
                    }
                });

                if (result.success)
                {
                    this.response.success = true;
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "UpdateProfile:200_SUCCESS");
                    this.response.data = new
                    {
                        USER_ID = user.UserId
                    };
                }
                else
                {
                    _logger.LogError("Frontend.AccountService.UpdateProfile.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "UpdateProfile:400_BAD_REQUEST");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.AccountService.UpdateProfile.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "UpdateProfile:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> ChangePassword(ChangePasswordParameter param)
        {
            try
            {
                var user = _dbContext.User.FirstOrDefault(f =>
                    f.UserId == _authen.User.USER_ID);

                if (user == null)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.NotFound;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "ChangePassword:404_USER_NOT_FOUND");

                    return this.response;
                }

                if (user.UserId != _authen.User.USER_ID)
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "ChangePassword:403_ACCESS_DENIED");

                    return this.response;
                }

                if (user.Password != HashExtension.Sha256(param.PASSWORD_OLD))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Conflict;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "ChangePassword:409_PASSWORD_NOT_MATCHING");

                    return this.response;
                }


                if (user.Password == HashExtension.Sha256(param.PASSWORD_NEW))
                {
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Conflict;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "ChangePassword:409_PASSWORD_NEW_MATCHING_PASSWORD_OLD");

                    return this.response;
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
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "ChangePassword:200_SUCCESS");
                }
                else
                {
                    _logger.LogError("Frontend.AccountService.ChangePassword.Transaction.Exception: {0}", result.exception.ToString());

                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.BadRequest;
                    this.response.message = _message.GetMessage(ROOT_MESSAGE, "ChangePassword:400_BAD_REQUEST");
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Frontend.AccountService.ChangePassword.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = _message.GetMessage(ROOT_MESSAGE, "ChangePassword:500_INTERNAL_SERVER_ERROR");
            }

            return await Task.Run(() => this.response);
        }
    }
}
