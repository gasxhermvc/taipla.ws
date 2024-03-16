using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.Restaurant;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurant;

        public RestaurantController(IRestaurantService culture)
        {
            DateTimeExtension.SetDateEnv();

            _restaurant = culture;
        }

        [HttpGet("restaurants")]
        public async Task<IActionResult> Restaurants([FromQuery] RestaurantRestaurantsParameter param)
        {
            var response = await _restaurant.Restaurants(param);

            return Ok(response);
        }

        [HttpGet("restaurant/{RES_ID}")]
        public async Task<IActionResult> GetRestaurant([FromQuery] RestaurantRestaurantsParameter param)
        {
            var response = await _restaurant.GetRestaurant(param);

            return Ok(response);
        }

        [HttpPost("created")]
        public async Task<IActionResult> Created([FromForm] RestaurantCreatedParameter param)
        {
            var response = await _restaurant.Created(param);

            return Ok(response);
        }

        [HttpPut("updated")]
        public async Task<IActionResult> Updated([FromForm] RestaurantUpdatedParameter param)
        {
            var response = await _restaurant.Updated(param);

            return Ok(response);
        }

        [HttpDelete("deleted")]
        public async Task<IActionResult> Deleted([FromQuery] RestaurantDeletedParameter param)
        {
            var response = await _restaurant.Deleted(param);

            return Ok(response);
        }

        [HttpGet("medias/{RES_ID}")]
        public async Task<IActionResult> Medias([FromQuery] RestaurantRestaurantsParameter param)
        {
            var response = await _restaurant.Medias(param);

            return Ok(response);
        }
    }
}
