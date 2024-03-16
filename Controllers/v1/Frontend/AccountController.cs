using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Frontend.Account;

namespace Taipla.Webservice.Controllers.v1.Frontend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            DateTimeExtension.SetDateEnv();
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterParameter param)
        {
            var response = await _accountService.Register(param);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var response = await _accountService.Profile();

            return Ok(response);
        }

        [Authorize]
        [HttpPut("profile/updated")]
        public async Task<IActionResult> UpdateProfile([FromForm] AccountProfileParameter param)
        {
            var response = await _accountService.UpdateProfile(param);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("profile/changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordParameter param)
        {
            var response = await _accountService.ChangePassword(param);

            return Ok(response);
        }
    }
}
