using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.Food;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _food;

        public FoodController(IFoodService culture)
        {
            DateTimeExtension.SetDateEnv();

            _food = culture;
        }

        [HttpGet("foods")]
        public async Task<IActionResult> Foods([FromQuery] FoodFoodsParameter param)
        {
            var response = await _food.Foods(param);

            return Ok(response);
        }

        [HttpGet("food/{FOOD_ID}")]
        public async Task<IActionResult> GetFood([FromQuery] FoodFoodsParameter param)
        {
            var response = await _food.GetFood(param);

            return Ok(response);
        }

        [HttpPost("created")]
        public async Task<IActionResult> Created([FromForm] FoodCreatedParameter param)
        {
            var response = await _food.Created(param);

            return Ok(response);
        }

        [HttpPut("updated")]
        public async Task<IActionResult> Updated([FromForm] FoodUpdatedParameter param)
        {
            var response = await _food.Updated(param);

            return Ok(response);
        }

        [HttpDelete("deleted")]
        public async Task<IActionResult> Deleted([FromQuery] FoodDeletedParameter param)
        {
            var response = await _food.Deleted(param);

            return Ok(response);
        }

        [HttpGet("medias/{FOOD_ID}")]
        public async Task<IActionResult> Medias([FromQuery] FoodFoodsParameter param)
        {
            var response = await _food.Medias(param);

            return Ok(response);
        }
    }
}
