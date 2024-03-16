using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Account;

namespace Taipla.Webservice.Controllers.v0.Frontend
{
    [ApiVersion("0")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IWebHostEnvironment _web;

        public AccountController(IWebHostEnvironment web)
        {
            _web = web;
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm] RegisterParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "account",
               "register.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "account",
               "profile.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPut("profile/updated")]
        public IActionResult UpdateProfile([FromForm] AccountProfileParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "account",
               "profile",
               "updated.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPut("profile/changepassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "account",
               "profile",
               "changepassword.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }
    }
}
