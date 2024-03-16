using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Auth;

namespace Taipla.Webservice.Controllers.v1.Frontend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth)
        {
            DateTimeExtension.SetDateEnv();

            _auth = auth;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginParameter param)
        {
            var response = await _auth.Login(param);

            return Ok(response);
        }

        [HttpGet("logout/{clientId}/{deviceId}")]
        public async Task<IActionResult> Logout([FromRoute] LogoutParameter param)
        {
            var response = await _auth.Logout(param);

            return Ok(response);
        }

        [HttpGet("userinfo")]
        public async Task<IActionResult> UserInfo()
        {
            var response = await _auth.UserInfo();

            return Ok(response);
        }
    }
}
