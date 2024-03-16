using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.Culture;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [ApiController]
    public class CultureController : ControllerBase
    {
        private readonly ICultureService _culture;

        public CultureController(ICultureService culture)
        {
            DateTimeExtension.SetDateEnv();

            _culture = culture;
        }

        [HttpGet("cultures")]
        public async Task<IActionResult> Cultures([FromQuery] CultureCulturesParameter param)
        {
            var response = await _culture.Cultures(param);

            return Ok(response);
        }

        [HttpGet("culture/{COUNTRY_ID}/{CULTURE_ID}")]
        public async Task<IActionResult> GetCulture([FromQuery] CultureCulturesParameter param)
        {
            var response = await _culture.GetCulture(param);

            return Ok(response);
        }

        [HttpPost("created")]
        public async Task<IActionResult> Created([FromForm] CultureCreatedParameter param)
        {
            var response = await _culture.Created(param);

            return Ok(response);
        }

        [HttpPut("updated")]
        public async Task<IActionResult> Updated([FromForm] CultureUpdatedParameter param)
        {
            var response = await _culture.Updated(param);

            return Ok(response);
        }

        [HttpDelete("deleted")]
        public async Task<IActionResult> Deleted([FromQuery] CultureDeletedParameter param)
        {
            var response = await _culture.Deleted(param);

            return Ok(response);
        }
    }
}
