using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Models.Parameters.Frontend.Food;

namespace Taipla.Webservice.Controllers.v1.Frontend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FoodController : ControllerBase
    {
        private readonly IFoodCenterService _foodCenterService;

        public FoodController(IFoodCenterService foodCenterService)
        {
            _foodCenterService = foodCenterService;
        }

        [HttpGet("detail/{foodId}")]
        public async Task<IActionResult> Detail([FromQuery] FoodDetailParameter param)
        {
            var response = await _foodCenterService.Detail(param);

            return Ok(response);
        }

        [HttpGet("detail/images/{foodId}")]
        public async Task<IActionResult> Images([FromQuery] FoodDetailParameter param)
        {
            var response = await _foodCenterService.Images(param);

            return Ok(response);
        }

        [HttpGet("legend/{foodId}")]
        public async Task<IActionResult> Legend([FromQuery] FoodDetailParameter param)
        {
            var response = await _foodCenterService.Legend(param);

            return Ok(response);
        }

        //[HttpGet("ingredients/{foodId}")]
        //public async Task<IActionResult> Ingredient([FromQuery] FoodDetailParameter param)
        //{
        //    var response = await _foodCenterService.Ingredient(param);

        //    return Ok(response);
        //}

        [HttpGet("recommendations/{foodId}")]
        public async Task<IActionResult> Recommendation([FromQuery] FoodDetailParameter param)
        {
            var response = await _foodCenterService.Recommendation(param);

            return Ok(response);
        }

        [HttpPost("vote/{foodId}")]
        public async Task<IActionResult> Vote([FromRoute] int foodId, [FromBody] FoodDetailVoteParameter param)
        {
            param.foodId = foodId;
            var response = await _foodCenterService.Vote(param);

            return Ok(response);
        }

        [HttpGet("vote/exist/{foodId}")]
        public async Task<IActionResult> VoteExist([FromQuery] FoodDetailVoteExistParameter param)
        {
            var response = await _foodCenterService.VoteExist(param);

            return Ok(response);
        }

        //[HttpGet("ingredient/legend/{foodId}/{ingredientId}")]
        //public async Task<IActionResult> IngredientLegend([FromQuery] FoodDetailIngredientLegendParameter param)
        //{
        //    var response = await _foodCenterService.IngredientLegend(param);

        //    return Ok(response);
        //}

        [HttpGet("comments/{foodId}")]
        public async Task<IActionResult> Comments([FromQuery] FoodDetailCommentParameter param)
        {
            var response = await _foodCenterService.Comments(param);

            return Ok(response);
        }

        [Produces("application/json")]
        [HttpPost("review/{foodId}")]
        public async Task<IActionResult> Review([FromForm] FoodDetailReviewParameter param)
        {
            var response = await _foodCenterService.Review(param);

            return Ok(response);
        }
    }
}
