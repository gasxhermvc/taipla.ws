using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Log;

namespace Taipla.Webservice.Controllers.v0.Frontend
{
    [ApiVersion("0")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly IWebHostEnvironment _web;

        public LogController(IWebHostEnvironment web)
        {
            _web = web;
        }

        [HttpPost("restaurant/{restaurantId}")]
        public IActionResult Restaurant([FromBody] LogRestaurantParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "logs",
               "restaurant.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPost("restaurant/menu/{restaurantId}/{menuId}")]
        public IActionResult RestaurantMenu([FromBody] LogRestaurantMenuParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "logs",
               "restaurant",
               "menu.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPost("restaurant/promotion/{restaurantId}/{promotionId}")]
        public IActionResult RestaurantPromotion([FromBody] LogRestaurantPromotionParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "logs",
               "restaurant",
               "promotion.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPost("food/{foodId}")]
        public IActionResult Food([FromBody] LogFoodParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "logs",
               "food.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }
    }
}
