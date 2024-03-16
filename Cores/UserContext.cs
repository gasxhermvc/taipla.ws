using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Taipla.Webservice.Entities;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Business.Enums;

namespace Taipla.Webservice.Cores
{
    public class UserInfo
    {
        public int USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
        public string ROLE { get; set; }
        public string CLIENT_ID { get; set; }
        public string AVATAR { get; set; }
        public string DISPLAY_NAME { get; set; }
        public int? RES_ID { get; set; }
        public RestaurantOwner? RES_OWNER { get; set; }
    }

    public class RestaurantOwner
    {
        public int RES_ID { get; set; }

        public int COUNTRY_ID { get; set; }

        public int OWNER_ID { get; set; }
    }


    public class UserContext<TUser> : JWTService<TUser>
        where TUser : UserInfo

    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public UserContext(IConfiguration configuration, IHttpContextAccessor context, TAIPLA_DbContext dbContext, IWebHostEnvironment env) : base(null, null)
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

                    User user = null;
                    if (authorization.Count > 0)
                    {
                        var token = authorization.ToString().Replace("Bearer ", string.Empty).Trim();
                        var userAccess = _dbContext.UserDevice.FirstOrDefault(f => f.Token == token &&
                            DateTime.Now < f.Expired);

                        if (userAccess != null)
                        {
                            user = _dbContext.User.FirstOrDefault(f => f.ClientId == userAccess.ClientId);
                        }
                    }
                    //var userId = int.Parse(UserContext.Claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier).Value);
                    //var username = UserContext.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name).Value;

                    //var user = _dbContext.User.FirstOrDefault(f => f.UserId == userId && f.Username == username);

                    if (user != null)
                    {
                        var avatar = _env.GetImageThumbnail(_context, user?.Avatar ?? string.Empty, ImageExtension.DEFAULT_AVATAR);

                        Restaurant res = null;
                        RestaurantOwner owner = null;
                        int? RES_ID = null;

                        if (user.Role == RoleEnum.OWNER.GetString())
                        {
                            res = _dbContext.Restaurant.Where(w => w.OwnerId == user.UserId)
                                //.Select(s => s.ResId)
                                .FirstOrDefault();

                            if (res != null)
                            {
                                RES_ID = res.ResId;
                                owner = new RestaurantOwner
                                {
                                    RES_ID = res.ResId,
                                    COUNTRY_ID = res.CountryId,
                                    OWNER_ID = user.UserId
                                };
                            }
                        }

                        if (user.Role == RoleEnum.STAFF.GetString())
                        {
                            var userParaent = _dbContext.User.FirstOrDefault(f => f.UserId == user.ParentId);
                            if (userParaent != null)
                            {
                                res = _dbContext.Restaurant.FirstOrDefault(w => w.OwnerId == userParaent.UserId);
                            }

                            if (res != null)
                            {
                                RES_ID = res.ResId;
                                owner = new RestaurantOwner
                                {
                                    RES_ID = res.ResId,
                                    COUNTRY_ID = res.CountryId,
                                    OWNER_ID = userParaent.UserId
                                };
                            }
                        }

                        this.user = new UserInfo
                        {
                            USER_ID = user.UserId,
                            USERNAME = user.Username,
                            EMAIL = user.Email,
                            FIRST_NAME = user.FirstName,
                            LAST_NAME = user.LastName,
                            PHONE = user.Phone,
                            ROLE = user.Role,
                            CLIENT_ID = user.ClientId,
                            AVATAR = avatar.image,
                            DISPLAY_NAME = string.Format("{0} {1}",
                               (user?.FirstName ?? string.Empty).Trim(),
                               (user?.LastName ?? string.Empty).Trim()).Trim(),
                            RES_ID = RES_ID,
                            RES_OWNER = owner
                        } as TUser;
                    }
                }
            }
        }
    }
}
