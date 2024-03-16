using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.Legend;
using Taipla.Webservice.Models.Parameters.Backend.Media;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class LegendController : ControllerBase
    {
        private ILegendService _legend;
        public LegendController(ILegendService media)
        {
            DateTimeExtension.SetDateEnv();
            _legend = media;
        }

        [HttpGet("legends")]
        public async Task<IActionResult> Legends([FromQuery] LegendLegendsParameter param)
        {
            var response = await _legend.Legends(param);

            return Ok(response);
        }

        [HttpGet("legend/{CODE}")]
        public async Task<IActionResult> GetLegend([FromQuery] LegendLegendsParameter param)
        {
            var response = await _legend.GetLegend(param);

            return Ok(response);
        }

        [HttpPost("created")]
        public async Task<IActionResult> Created([FromForm] LegendCreatedParameter param)
        {
            var response = await _legend.Created(param);

            return Ok(response);
        }

        [HttpPut("updated")]
        public async Task<IActionResult> Updated([FromForm] LegendUpdatedParameter param)
        {
            var response = await _legend.Updated(param);

            return Ok(response);
        }

        [HttpDelete("deleted")]
        public async Task<IActionResult> Deleted([FromQuery] LegendDeletedParameter param)
        {
            var response = await _legend.Deleted(param);

            return Ok(response);
        }
    }
}
