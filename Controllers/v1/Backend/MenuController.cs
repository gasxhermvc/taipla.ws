using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Helpers.JWTAuthentication;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IAuthenticationService<UserInfo> _authen;

        private readonly IWebHostEnvironment _web;

        private BaseResponse response = new BaseResponse();
        public MenuController(IAuthenticationService<UserInfo> authen
            , IWebHostEnvironment web)
        {
            DateTimeExtension.SetDateEnv();
            _authen = authen;
            _web = web;
        }

        [HttpGet("lists")]
        public IActionResult Menu([FromQuery] string ROLE)
        {
            if ((this._authen.User.ROLE != RoleEnum.SUPER_ADMIN.GetString() &&
                this._authen.User.ROLE != RoleEnum.ADMIN.GetString()) &&
                !string.IsNullOrEmpty(ROLE))
            {

                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.Forbidden;
                this.response.message = "ดึงข้อมูลไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงระบบ";
                this.response.data = null;

                return Ok(this.response);
            }

            string roleAccess = !string.IsNullOrEmpty(ROLE) ? ROLE : this._authen.User.ROLE;
            string menu = string.Empty;

            switch (roleAccess)
            {
                case "super_admin":
                    menu = "SuperAdmin";
                    break;
                case "admin":
                    menu = "Admin";
                    break;
                case "post":
                    menu = "Post";
                    break;
                case "post_restaurant":
                    menu = "PostRestaurant";
                    break;
                case "owner":
                    menu = "Owner";
                    break;
                case "staff":
                    menu = "Staff";
                    break;
                case "client":
                default:
                    this.response.success = false;
                    this.response.statusCode = (int)HttpStatusCode.Forbidden;
                    this.response.message = "ดึงข้อมูลไม่สำเร็จ, เนื่องจากไม่มีสิทธิ์เข้าถึงระบบ";
                    this.response.data = null;

                    return Ok(this.response);
            }

            string menuFile = string.Format("{0}/Business/Jsons/Menus/{1}Menu.json"
                , PathExtension.BasePath(_web)
                , menu);

            if (!System.IO.File.Exists(menuFile))
            {
                this.response.success = false;
                this.response.statusCode = (int)HttpStatusCode.NotFound;
                this.response.message = "ดึงข้อมูลไม่สำเร็จ, เนื่องจากไม่พบไฟล์ Menu";
                this.response.data = null;

                return Ok(this.response);
            }

            var jsonFile = System.IO.File.ReadAllText(menuFile, System.Text.Encoding.UTF8);

            var data = JsonConvert.DeserializeObject<List<object>>(jsonFile);

            this.response.success = true;
            this.response.statusCode = (int)HttpStatusCode.OK;
            this.response.message = "ดึงข้อมูลสำเร็จ";
            this.response.total = data.Count;
            this.response.data = data;

            return Ok(this.response);
        }
    }
}
