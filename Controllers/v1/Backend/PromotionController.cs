using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.Promotion;
using Taipla.Webservice.Models.Parameters.Backend.Media;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private IPromotionService _promotion;
        public PromotionController(IPromotionService media)
        {
            DateTimeExtension.SetDateEnv();
            _promotion = media;
        }

        [HttpGet("promotions")]
        public async Task<IActionResult> Promotions([FromQuery] PromotionPromotionsParameter param)
        {
            var response = await _promotion.Promotions(param);

            return Ok(response);
        }

        [HttpGet("promotion/{RES_ID}")]
        public async Task<IActionResult> GetPromotion([FromQuery] PromotionPromotionsParameter param)
        {
            var response = await _promotion.GetPromotion(param);

            return Ok(response);
        }

        [HttpPost("created")]
        public async Task<IActionResult> Created([FromForm] PromotionCreatedParameter param)
        {
            var response = await _promotion.Created(param);

            return Ok(response);
        }

        [HttpPut("updated")]
        public async Task<IActionResult> Updated([FromForm] PromotionUpdatedParameter param)
        {
            var response = await _promotion.Updated(param);

            return Ok(response);
        }

        [HttpDelete("deleted")]
        public async Task<IActionResult> Deleted([FromQuery] PromotionDeletedParameter param)
        {
            var response = await _promotion.Deleted(param);

            return Ok(response);
        }
    }
}
