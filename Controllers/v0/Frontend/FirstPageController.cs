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
using Taipla.Webservice.Models.Parameters.Frontend.FirstPage;

namespace Taipla.Webservice.Controllers.v0.Frontend
{
    [ApiVersion("0")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class FirstPageController : ControllerBase
    {
        private readonly IWebHostEnvironment _web;

        public FirstPageController(IWebHostEnvironment web)
        {
            _web = web;
        }

        [HttpGet("foods")]
        public IActionResult Foods([FromQuery] FirstPageFoodParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "firstpage",
               "foods.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("food/topviewer")]
        public IActionResult FoodTopViewer([FromQuery] FirstPageFoodTopViewerParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "firstpage",
               "food",
               "topviewer.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("food/categories")]
        public IActionResult Categories([FromQuery] FirstPageFoodCategoriesParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "firstpage",
               "food",
               "categories.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }


        [HttpGet("food/cultures")]
        public IActionResult Cultures([FromQuery] FirstPageFoodCulturesParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "firstpage",
               "food",
               "foodcultures.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("restaurant/topviewer")]
        public IActionResult RestaurantTopViewer([FromQuery] FirstPageRestaurantParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "firstpage",
               "restaurant",
               "topviewer.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("restaurant/promotions")]
        public IActionResult RestuarantPromotions([FromQuery] FirstPageRestaurantParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "firstpage",
               "restaurant",
               "promotions.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("restaurant/near")]
        public IActionResult RestuarantNear([FromQuery] FirstPageRestaurantNearParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "firstpage",
               "restaurant",
               "near.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }
    }
}
