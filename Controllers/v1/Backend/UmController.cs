using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.Um;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [ApiController]
    public class UmController : ControllerBase
    {
        private readonly IUmService _um;

        public UmController(IUmService um)
        {
            DateTimeExtension.SetDateEnv();

            _um = um;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Users([FromQuery] UmUserParameter param)
        {
            var response = await _um.Users(param);

            return Ok(response);
        }


        [HttpGet("user/{USER_ID}")]
        public async Task<IActionResult> GetUser([FromQuery] UmUserParameter param)
        {
            var response = await _um.GetUser(param);

            return Ok(response);
        }

        [HttpPost("created")]
        public async Task<IActionResult> Created([FromForm] UmCreatedParameter param)
        {
            var response = await _um.Created(param);

            return Ok(response);
        }

        [HttpPut("updated")]
        public async Task<IActionResult> Updated([FromForm] UmUpdatedParameter param)
        {
            var response = await _um.Updated(param);

            return Ok(response);
        }

        [HttpDelete("deleted")]
        public async Task<IActionResult> Deleted([FromQuery] UmDeletedParameter param)
        {
            var response = await _um.Deleted(param);

            return Ok(response);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UmChangePasswordParameter param)
        {
            var response = await _um.ChangePassword(param);

            return Ok(response);
        }
    }
}
