using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Helpers.JWTAuthentication;

namespace Taipla.Webservice.Cores
{
    public class ClientInfo
    {
        [JsonProperty("userId")]
        public int USER_ID { get; set; }
        [JsonProperty("username")]
        public string USERNAME { get; set; }
        [JsonProperty("firstName")]
        public string FIRST_NAME { get; set; }
        [JsonProperty("lastName")]
        public string LAST_NAME { get; set; }
        [JsonProperty("phoneNumber")]
        public string PHONE { get; set; }
        [JsonProperty("email")]
        public string EMAIL { get; set; }
        [JsonProperty("role")]
        public string ROLE { get; set; }
        [JsonProperty("clientId")]
        public string CLIENT_ID { get; set; }
        [JsonProperty("imageAvatar")]
        public string IMAGE_AVATAR { get; set; }
        [JsonProperty("imageAvatarSM")]
        public string IMAGE_AVATAR_SM { get; set; }
        [JsonProperty("imageAvatarMD")]
        public string IMAGE_AVATAR_MD { get; set; }
        [JsonProperty("imageAvatarLG")]
        public string IMAGE_AVATAR_LG { get; set; }
        [JsonProperty("deviceId")]
        [JsonIgnore]
        public string DEVICE_ID { get; set; }
    }

    public class ClientContext<TUser> : JWTService<TUser>
        where TUser : ClientInfo

    {
        private readonly TAIPLA_DbContext _dbContext;

        private readonly IWebHostEnvironment _env;

        public ClientContext(IConfiguration configuration, IHttpContextAccessor context, TAIPLA_DbContext dbContext, IWebHostEnvironment env) : base(null, null)
        {
            _configuration = configuration;
            _context = context;
            _dbContext = dbContext;
            _env = env;
        }

        private TUser user = null;

        public override TUser User
        {
            get
            {
                this.GetUserInfo();
                return this.user;
            }
        }

        private void GetUserInfo()
        {

            if (this.user == null)
            {
                var UserContext = this.UserContext();
                if (UserContext.Claims != null && UserContext.Claims.Count() > 0)
                {
                    StringValues authorization;
                    _context.HttpContext.Request.Headers.TryGetValue("Authorization", out authorization);

                    string token = string.Empty;
                    User user = null;
                    if (authorization.Count > 0)
                    {
                        token = authorization.ToString().Replace("Bearer ", string.Empty).Trim();
                        var userAccess = _dbContext.UserDevice.FirstOrDefault(f => f.Token == token &&
                            DateTime.Now < f.Expired);

                        if (userAccess != null)
                        {
                            user = _dbContext.User.FirstOrDefault(f => f.ClientId == userAccess.ClientId);
                        }
                    }

                    if (user != null)
                    {
                        Media media = _dbContext.UploadFileUtilities.GetMedia(
                            UploadEnum.CLIENT.GetString(),
                            user.UserId.ToString(),
                            user.Avatar);

                        this.user = new ClientInfo
                        {
                            USER_ID = user.UserId,
                            USERNAME = user.Username,
                            EMAIL = user.Email,
                            FIRST_NAME = user.FirstName,
                            LAST_NAME = user.LastName,
                            PHONE = user.Phone,
                            ROLE = user.Role,
                            CLIENT_ID = user.ClientId
                        } as TUser;

                        ImageResize image = null;

                        if (media != null)
                        {
                            image = _env.GetImage(_context, media);

                            if (image != null)
                            {
                                this.user.IMAGE_AVATAR = image.image;
                                this.user.IMAGE_AVATAR_SM = image.imageSM;
                                this.user.IMAGE_AVATAR_MD = image.imageMD;
                                this.user.IMAGE_AVATAR_LG = image.imageLG;
                            }
                        }

                        //=>device
                        var device = _dbContext.UserUtilities.GetDevice(token);
                        if (device != null)
                        {
                            this.user.DEVICE_ID = device.DeviceId;
                        }
                    }
                }
            }
        }
    }
}
