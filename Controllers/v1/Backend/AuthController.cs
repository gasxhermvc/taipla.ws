using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.Auth;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [Produces("application/json")]
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

        [HttpPost("logout/{CLIENT_ID}")]
        public async Task<IActionResult> Logout([FromRoute] string CLIENT_ID)
        {
            var response = await _auth.Logout(CLIENT_ID);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("userinfo")]
        public async Task<IActionResult> UserInfo()
        {
            var response = await _auth.UserInfo();

            return Ok(response);
        }

        [HttpDelete("Test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                Suucess = "OK"
            });
        }
    }
}
