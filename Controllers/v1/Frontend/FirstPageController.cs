using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Frontend.FirstPage;

namespace Taipla.Webservice.Controllers.v1.Frontend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class FirstPageController : ControllerBase
    {
        private readonly IFirstPageService _firstPageService;

        public FirstPageController(IFirstPageService firstPageService)
        {
            DateTimeExtension.SetDateEnv();
            _firstPageService = firstPageService;
        }

        [HttpGet("foods")]
        public async Task<IActionResult> Foods([FromQuery] FirstPageFoodParameter param)
        {
            var response = await _firstPageService.Foods(param);

            return Ok(response);
        }

        [HttpGet("food/topviewer")]
        public async Task<IActionResult> FoodTopViewer([FromQuery] FirstPageFoodTopViewerParameter param)
        {
            var response = await _firstPageService.FoodTopViewer(param);

            return Ok(response);
        }

        [HttpGet("food/categories")]
        public async Task<IActionResult> Categories([FromQuery] FirstPageFoodCategoriesParameter param)
        {
            var response = await _firstPageService.Categories(param);

            return Ok(response);
        }


        [HttpGet("food/cultures")]
        public async Task<IActionResult> Cultures([FromQuery] FirstPageFoodCulturesParameter param)
        {
            var response = await _firstPageService.Cultures(param);

            return Ok(response);
        }

        [HttpGet("restaurant/topviewer")]
        public async Task<IActionResult> RestaurantTopViewer([FromQuery] FirstPageRestaurantParameter param)
        {
            var response = await _firstPageService.RestaurantTopViewer(param);

            return Ok(response);
        }

        [HttpGet("restaurant/promotions")]
        public async Task<IActionResult> RestaurantPromotions([FromQuery] FirstPageRestaurantParameter param)
        {
            var response = await _firstPageService.RestaurantPromotions(param);

            return Ok(response);
        }

        [HttpGet("restaurant/near")]
        public async Task<IActionResult> RestaurantNear([FromQuery] FirstPageRestaurantNearParameter param)
        {
            var response = await _firstPageService.RestaurantNear(param);

            return Ok(response);
        }
    }
}
