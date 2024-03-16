using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.RestaurantMenu;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/restaurant-menu")]
    [ApiController]
    public class RestaurantMenuController : ControllerBase
    {
        private readonly IRestaurantMenuService _RestaurantMenu;

        public RestaurantMenuController(IRestaurantMenuService culture)
        {
            DateTimeExtension.SetDateEnv();

            _RestaurantMenu = culture;
        }

        [HttpGet("restaurant-menus")]
        public async Task<IActionResult> RestaurantMenus([FromQuery] RestaurantMenuRestaurantMenusParameter param)
        {
            var response = await _RestaurantMenu.RestaurantMenus(param);

            return Ok(response);
        }

        [HttpGet("restaurant-menu")]
        public async Task<IActionResult> GetRestaurantMenu([FromQuery] RestaurantMenuRestaurantMenusParameter param)
        {
            var response = await _RestaurantMenu.GetRestaurantMenu(param);

            return Ok(response);
        }

        [HttpPost("created")]
        public async Task<IActionResult> Created([FromForm] RestaurantMenuCreatedParameter param)
        {
            var response = await _RestaurantMenu.Created(param);

            return Ok(response);
        }

        [HttpPut("updated")]
        public async Task<IActionResult> Updated([FromForm] RestaurantMenuUpdatedParameter param)
        {
            var response = await _RestaurantMenu.Updated(param);

            return Ok(response);
        }

        [HttpDelete("deleted")]
        public async Task<IActionResult> Deleted([FromQuery] RestaurantMenuDeletedParameter param)
        {
            var response = await _RestaurantMenu.Deleted(param);

            return Ok(response);
        }

        [HttpGet("medias/{RES_ID}")]
        public async Task<IActionResult> Medias([FromQuery] RestaurantMenuRestaurantMenusParameter param)
        {
            var response = await _RestaurantMenu.Medias(param);

            return Ok(response);
        }
    }
}
