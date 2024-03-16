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
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Extensions;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class LUTService : ILUTService
    {
        private readonly TAIPLA_DbContext _dbContext;
        private readonly ILogger _logger;
        private readonly IAuthenticationService<UserInfo> _authen;

        private BaseResponse response = new BaseResponse();

        public LUTService(TAIPLA_DbContext dbContext, ILoggerFactory logger, IAuthenticationService<UserInfo> authen)
        {
            _dbContext = dbContext;
            _logger = logger.CreateLogger("Service");
            _authen = authen;
        }

        public async Task<BaseResponse> Countries(int? COUNTRY_ID)
        {
            try
            {
                var query = _dbContext.FoodCountry.AsQueryable();

                if (COUNTRY_ID != null && COUNTRY_ID > 0)
                {
                    query.Where(w => w.CountryId == COUNTRY_ID);
                }

                var foodCountries = query.ToList();

                var responseData = foodCountries.Select(s => new
                {
                    CODE = s.CountryId,
                    DESCR = s.Description,
                    VALUE_TH = s.NameTh,
                    VALUE_EN = s.NameEn,
                }).ToList();

                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.Countries.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }


            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Cultures(int? COUNTRY_ID, int? CULTURE_ID)
        {
            try
            {
                var query = _dbContext.FoodCulture.AsQueryable();

                if (COUNTRY_ID != null && COUNTRY_ID > 0)
                {
                    query.Where(w => w.CountryId == COUNTRY_ID);
                }

                if (CULTURE_ID != null && CULTURE_ID > 0)
                {
                    query.Where(w => w.CultureId == CULTURE_ID);
                }

                var foodCultures = query.ToList();

                var responseData = foodCultures.Select(s => new
                {
                    COUNTRY_ID = s.CountryId,
                    CODE = s.CultureId,
                    DESCR = s.Description,
                    VALUE_TH = s.NameTh,
                    VALUE_EN = s.NameEn,
                }).ToList();

                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.Cultures.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }


            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Roles()
        {
            try
            {
                List<object> responseData = new List<object>();

                if (_authen.IsAuthenticated)
                {
                    switch (_authen.User.ROLE)
                    {
                        case "super_admin":
                            responseData = new List<object>
                            {
                                new { CODE = "admin", DESCR = "Admin" },
                                new { CODE = "post", DESCR = "Post" },
                                new { CODE = "post_restaurant", DESCR = "Post Restaurant" },
                                new { CODE = "owner", DESCR = "Owner" },
                                new { CODE = "staff", DESCR = "Staff" },
                                new { CODE = "client", DESCR = "Client" }
                            };
                            break;

                        case "admin":
                            responseData = new List<object>
                            {
                                new { CODE = "post", DESCR = "Post" },
                                new { CODE = "post_restaurant", DESCR = "Post Restaurant" },
                                new { CODE = "owner", DESCR = "Owner" },
                                new { CODE = "staff", DESCR = "Staff" },
                                new { CODE = "client", DESCR = "Client" }
                            };
                            break;

                        case "owner":
                            responseData = new List<object>
                            {
                                new { CODE = "staff", DESCR = "Staff" }
                            };
                            break;
                    }
                }

                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.Roles.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }


            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> LegendTypes(int? LEGEND_TYPE)
        {
            try
            {
                List<object> responseData = new List<object>();

                switch (LEGEND_TYPE)
                {
                    case 0: //=>อาหารส่วนกลาง
                        responseData = new List<object>
                            {
                                new { CODE = "1", DESCR = LegendEnum.LEGEND_FOOD.GetString() },
                            };
                        break;

                    case 1:
                        responseData = new List<object>
                            {
                                new { CODE = "2", DESCR = LegendEnum.LEGEND_RESTAURANT.GetString() },
                            };
                        break;
                    default:
                        responseData = new List<object>
                            {
                                new { CODE = "1", DESCR = LegendEnum.LEGEND_FOOD.GetString() },
                                new { CODE = "2", DESCR = LegendEnum.LEGEND_RESTAURANT.GetString() },
                            };
                        break;
                }

                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.LegendType.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Owners()
        {
            try
            {
                List<object> responseData = new List<object>();

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST:
                    case RoleEnum.POST_RESTAURANT:
                        break;
                    default:
                        this.response.success = true;
                        this.response.statusCode = (int)HttpStatusCode.NoContent;
                        this.response.message = "ดึงข้อมูลสำเร็จ, แต่ไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = new List<object>();
                        return this.response;
                }

                var owners = _dbContext.User.Where(w => w.Role == RoleEnum.OWNER.GetString()).ToList();
                responseData = (from user in owners
                                let fullName = string.Format("{0} {1}",
                                    user.FirstName.Trim()?.FirstCharToUpper() ?? string.Empty,
                                    user.LastName.Trim()?.FirstCharToUpper() ?? string.Empty).Trim()
                                let fullUser = string.Format("Username: {0}({1})",
                                    user.Username.Trim(),
                                    !string.IsNullOrEmpty(fullName) ? fullName : "-").Trim()
                                select new
                                {
                                    CODE = user.UserId,
                                    DESCR = fullUser,
                                    FULL_NAME = fullName
                                } as object).ToList();


                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.Owners.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Staff(int? PARENT_ID)
        {
            try
            {
                List<object> responseData = new List<object>();
                var staffQuery = _dbContext.User.Where(w => w.Role == RoleEnum.STAFF.GetString()).AsQueryable();
                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                    case RoleEnum.POST:
                        if (PARENT_ID != null)
                        {
                            staffQuery = staffQuery.Where(w => w.ParentId == PARENT_ID);
                        }
                        break;
                    case RoleEnum.OWNER:
                        staffQuery = staffQuery.Where(w => w.ParentId == _authen.User.USER_ID);
                        break;
                    default:
                        this.response.success = true;
                        this.response.statusCode = (int)HttpStatusCode.NoContent;
                        this.response.message = "ดึงข้อมูลสำเร็จ, แต่ไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = new List<object>();
                        return this.response;
                }

                var staff = staffQuery.ToList();
                responseData = (from user in staff
                                let fullName = string.Format("{0} {1}",
                                    user.FirstName.Trim()?.FirstCharToUpper() ?? string.Empty,
                                    user.LastName.Trim()?.FirstCharToUpper() ?? string.Empty).Trim()
                                let fullUser = string.Format("Username: {0}({1})",
                                    user.Username.Trim(),
                                    !string.IsNullOrEmpty(fullName) ? fullName : "-").Trim()
                                select new
                                {
                                    CODE = user.UserId,
                                    DESCR = fullUser,
                                    FULL_NAME = fullName
                                } as object).ToList();


                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.Staff.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> PromotionTypes()
        {
            try
            {
                List<object> responseData = new List<object>();


                responseData = new List<object>
                            {
                                new { CODE = "0", DESCR = PromotionEnum.HIDE.GetString() },
                                new { CODE = "1", DESCR = PromotionEnum.USE_ESTIMATE.GetString() },
                                new { CODE = "2", DESCR = PromotionEnum.FREEZE.GetString() },
                            };

                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.PromotionTypes.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> AuthorCreateRestaurant()
        {
            try
            {
                List<object> responseData = new List<object>();

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                        break;
                    default:
                        this.response.success = true;
                        this.response.statusCode = (int)HttpStatusCode.NoContent;
                        this.response.message = "ดึงข้อมูลสำเร็จ, แต่ไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = new List<object>();
                        return this.response;
                }

                var authorRes = _dbContext.Restaurant.Where(w => w.UserId > 0)
                    .Select(s => new
                    {
                        UserId = s.UserId
                    }).ToList();

                var authorIds = authorRes.Select(s => s.UserId).Distinct().ToList();

                var authors = _dbContext.User.Where(w => authorIds.Contains(w.UserId)).ToList();
                responseData = (from author in authors
                                let fullName = string.Format("{0} {1}",
                                    author.FirstName?.Trim().FirstCharToUpper() ?? string.Empty,
                                    author.LastName?.Trim().FirstCharToUpper() ?? string.Empty).Trim()
                                let fullUser = string.Format("Username: {0}({1})",
                                    author.Username.Trim(),
                                    !string.IsNullOrEmpty(fullName) ? fullName : "-").Trim()
                                select new
                                {
                                    CODE = author.UserId,
                                    DESCR = fullUser,
                                    FULL_NAME = fullName
                                } as object).ToList();


                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.AuthorCreateRestaurant.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> AuthorCreateFoodCenter()
        {
            try
            {
                List<object> responseData = new List<object>();

                var role = _authen.User.ROLE.ParseEnum<RoleEnum>(RoleEnum.UNKNOW);

                switch (role)
                {
                    case RoleEnum.SUPER_ADMIN:
                    case RoleEnum.ADMIN:
                        break;
                    default:
                        this.response.success = true;
                        this.response.statusCode = (int)HttpStatusCode.NoContent;
                        this.response.message = "ดึงข้อมูลสำเร็จ, แต่ไม่มีสิทธิ์เข้าถึงข้อมูล";
                        this.response.data = new List<object>();
                        return this.response;
                }

                var authorFood = _dbContext.FoodCenter.Where(w => w.UserId > 0)
                    .Select(s => new
                    {
                        UserId = s.UserId
                    }).ToList();

                var authorIds = authorFood.Select(s => s.UserId).Distinct().ToList();

                var authors = _dbContext.User.Where(w => authorIds.Contains(w.UserId)).ToList();
                responseData = (from author in authors
                                let fullName = string.Format("{0} {1}",
                                    author.FirstName?.Trim().FirstCharToUpper() ?? string.Empty,
                                    author.LastName?.Trim().FirstCharToUpper() ?? string.Empty).Trim()
                                let fullUser = string.Format("Username: {0}({1})",
                                    author.Username.Trim(),
                                    !string.IsNullOrEmpty(fullName) ? fullName : "-").Trim()
                                select new
                                {
                                    CODE = author.UserId,
                                    DESCR = fullUser,
                                    FULL_NAME = fullName
                                } as object).ToList();


                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.AuthorCreateFoodCenter.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }

            return await Task.Run(() => this.response);
        }

        public async Task<BaseResponse> Provinces()
        {
            try
            {
                List<object> responseData = new List<object>();

                responseData = new List<object>
                {
                    new { CODE = "นครศรีธรรมราช", DESCR = "นครศรีธรรมราช" },
                    new { CODE = "ภูเก็ต", DESCR = "ภูเก็ต" },
                    new { CODE = "สงขลา", DESCR = "สงขลา" },
                };

                this.response.success = true;
                this.response.total = responseData.Count;

                if (responseData != null && responseData.Count > 0)
                {
                    this.response.statusCode = (int)HttpStatusCode.OK;
                    this.response.message = "ดึงข้อมูลสำเร็จ";
                }
                else
                {
                    this.response.statusCode = (int)HttpStatusCode.NoContent;
                    this.response.message = "ดึงข้อมูลสำเร็จ แต่ไม่พบข้อมูล";
                }

                this.response.data = responseData;
            }
            catch (Exception e)
            {
                _logger.LogError("LUTService.Provinces.Exception: {0}", e.ToString());

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.InternalServerError;
                this.response.message = "ไม่สำเร็จ, เนื่องจากเกิดข้อผิดพลาดในระบบ";
                this.response.data = new List<object>();
            }


            return await Task.Run(() => this.response);
        }
    }
}
