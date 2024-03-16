using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Models.Parameters.Frontend.Log;

namespace Taipla.Webservice.Controllers.v1.Frontend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpPost("restaurant/{restaurantId}")]
        public async Task<IActionResult> Restaurant([FromRoute] int restaurantId, LogRestaurantParameter param)
        {
            param.restaurantId = restaurantId;

            var response = await _logService.Restaurant(param);

            return Ok(response);
        }

        [HttpPost("restaurant/menu/{restaurantId}/{menuId}")]
        public async Task<IActionResult> RestaurantMenu([FromRoute] int restaurantId,
            [FromRoute] int menuId,
            LogRestaurantMenuParameter param)
        {
            param.restaurantId = restaurantId;
            param.menuId = menuId;

            var response = await _logService.RestaurantMenu(param);

            return Ok(response);
        }

        [HttpPost("restaurant/promotion/{restaurantId}/{promotionId}")]
        public async Task<IActionResult> RestaurantPromotion([FromRoute] int restaurantId,
            [FromRoute] int promotionId,
            LogRestaurantPromotionParameter param)
        {
            param.restaurantId = restaurantId;
            param.promotionId = promotionId;

            var response = await _logService.RestaurantPromotion(param);

            return Ok(response);
        }

        [HttpPost("food/{foodId}")]
        public async Task<IActionResult> Food([FromRoute] int foodId, LogFoodParameter param)
        {
            param.foodId = foodId;
            var response = await _logService.Food(param);

            return Ok(response);
        }
    }
}
