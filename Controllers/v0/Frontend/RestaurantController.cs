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
using Taipla.Webservice.Models.Parameters.Frontend.Restaurant;

namespace Taipla.Webservice.Controllers.v0.Frontend
{
    [ApiVersion("0")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IWebHostEnvironment _web;

        public RestaurantController(IWebHostEnvironment web)
        {
            _web = web;
        }

        [HttpGet("detail/{restaurantId}")]
        public IActionResult Detail([FromQuery] RestaurantDetailParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "restaurant",
               "detail.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("menu/{restaurantId}")]
        public IActionResult Menu([FromQuery] RestaurantMenuParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "restaurant",
               "menu.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("menu/detail/{restaurantId}/{menuId}")]
        public IActionResult MenuDetail([FromQuery] RestaurantMenuDetailParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "restaurant",
               "menu",
               "detail.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("menu/ingredients/{restaurantId}/{menuId}")]
        public IActionResult MenuIngredient([FromQuery] RestaurantMenuIngredientParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "restaurant",
               "menu",
               "ingredients.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPost("vote/{restuarantId}")]
        public IActionResult Vote([FromBody] RestaurantDetailVoteParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "restaurant",
               "vote.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("vote/exist/{restaurantId}")]
        public IActionResult VoteExist([FromQuery] RestaurantDetailParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "restaurant",
               "vote_exist.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("comments/{restaurantId}")]
        public IActionResult Comments([FromQuery] RestaurantDetailCommentParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "restaurant",
               "comments.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [Produces("application/json")]
        [HttpPost("review/{restaurantId}")]
        public IActionResult Review([FromForm] RestaurantDetailReviewParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "restaurant",
               "review.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }


        [HttpGet("promotions/{restaurantId}")]
        public IActionResult Promotion([FromQuery] RestaurantPromotionParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "restaurant",
               "promotions.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

    }
}
