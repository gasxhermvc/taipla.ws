using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Models.Parameters.Frontend.Restaurant;

namespace Taipla.Webservice.Controllers.v1.Frontend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet("detail/{restaurantId}")]
        public async Task<IActionResult> Detail([FromRoute] int restaurantId, [FromQuery] RestaurantParameter param)
        {
            param.RESTAURANT_ID = restaurantId;

            var response = await _restaurantService.Detail(param);

            return Ok(response);
        }

        [HttpGet("menu/{restaurantId}")]
        public async Task<IActionResult> Menu([FromRoute] int restaurantId, [FromQuery] RestaurantParameter param)
        {
            param.RESTAURANT_ID = restaurantId;

            var response = await _restaurantService.Menu(param);

            return Ok(response);
        }

        [HttpGet("menu/detail/{restaurantId}/{menuId}")]
        public async Task<IActionResult> MenuDetail([FromRoute] int restaurantId,
            [FromRoute] int menuId, [FromQuery] RestaurantMenuDetailParameter param)
        {
            param.RESTAURANT_ID = restaurantId;
            param.MENU_ID = menuId;

            var response = await _restaurantService.MenuDetail(param);

            return Ok(response);
        }

        //[HttpGet("menu/ingredients/{restaurantId}/{menuId}")]
        //public async Task<IActionResult> MenuIngredient([FromRoute] int restaurantId,
        //    [FromRoute] int menuId, [FromQuery] RestaurantMenuIngredientParameter param)
        //{
        //    param.RESTAURANT_ID = restaurantId;
        //    param.MENU_ID = menuId;

        //    var response = await _restaurantService.MenuIngredient(param);

        //    return Ok(response);
        //}

        [HttpGet("legend/{restaurantId}/{menuId}")]
        public async Task<IActionResult> Legend([FromQuery] RestaurantMenuDetailParameter param)
        {
            var response = await _restaurantService.Legend(param);

            return Ok(response);
        }

        [HttpPost("vote/{restaurantId}")]
        public async Task<IActionResult> Vote([FromRoute] int restaurantId, RestaurantDetailVoteParameter param)
        {
            param.RESTAURANT_ID = restaurantId;

            var response = await _restaurantService.Vote(param);

            return Ok(response);
        }

        [HttpGet("vote/exist/{restaurantId}")]
        public async Task<IActionResult> VoteExist([FromRoute] int restaurantId, [FromQuery] RestaurantDetailParameter param)
        {
            param.RESTAURANT_ID = restaurantId;

            var response = await _restaurantService.VoteExist(param);

            return Ok(response);
        }

        [HttpGet("comments/{restaurantId}")]
        public async Task<IActionResult> Comments([FromRoute] int restaurantId, [FromQuery] RestaurantDetailCommentParameter param)
        {
            param.RESTAURANT_ID = restaurantId;

            var response = await _restaurantService.Comments(param);

            return Ok(response);
        }

        [Authorize]
        [Produces("application/json")]
        [HttpPost("review/{restaurantId}")]
        public async Task<IActionResult> Review([FromRoute] int restaurantId, [FromForm] RestaurantDetailReviewParameter param)
        {
            param.RESTAURANT_ID = restaurantId;

            var response = await _restaurantService.Review(param);

            return Ok(response);
        }


        [HttpGet("promotions/{restaurantId}")]
        public async Task<IActionResult> Promotion([FromRoute] int restaurantId, [FromQuery] RestaurantPromotionParameter param)
        {
            param.RESTAURANT_ID = restaurantId;

            var response = await _restaurantService.Promotion(param);
            return Ok(response);
        }

    }
}
