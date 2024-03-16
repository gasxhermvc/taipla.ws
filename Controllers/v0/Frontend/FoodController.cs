using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Food;

namespace Taipla.Webservice.Controllers.v0.Frontend
{
    [ApiVersion("0")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FoodController : ControllerBase
    {
        private readonly IWebHostEnvironment _web;

        public FoodController(IWebHostEnvironment web)
        {
            _web = web;
        }


        [HttpGet("detail/{foodId}")]
        public IActionResult Detail([FromQuery] FoodDetailParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "detail.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            JObject jObj = JObject.FromObject(response.data);

            //image
            List<string> _image = JsonConvert.DeserializeObject<List<string>>(jObj["image"].ToString());
            var image = _image.Take(param.limit).ToList();
            jObj["image"] = JToken.FromObject(image);

            //imageSM
            List<string> _imageSM = JsonConvert.DeserializeObject<List<string>>(jObj["imageSM"].ToString());
            var imageSM = _imageSM.Take(param.limit).ToList();
            jObj["imageSM"] = JToken.FromObject(imageSM);

            //imageMD
            List<string> _imageMD = JsonConvert.DeserializeObject<List<string>>(jObj["imageMD"].ToString());
            var imageMD = _imageMD.Take(param.limit).ToList();
            jObj["imageMD"] = JToken.FromObject(imageMD);

            //imageLG
            List<string> _imageLG = JsonConvert.DeserializeObject<List<string>>(jObj["imageLG"].ToString());
            var imageLG = _imageLG.Take(param.limit).ToList();
            jObj["imageLG"] = JToken.FromObject(imageLG);

            response.total = image.Count;
            response.data = jObj.ToObject<object>();

            return Ok(response);
        }

        [HttpGet("detail/images/{foodId}")]
        public IActionResult Images([FromQuery] FoodDetailParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "images.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            JObject jObj = JObject.FromObject(response.data);

            //image
            List<string> _image = JsonConvert.DeserializeObject<List<string>>(jObj["image"].ToString());
            var image = _image.Take(param.limit).ToList();
            jObj["image"] = JToken.FromObject(image);

            //imageSM
            List<string> _imageSM = JsonConvert.DeserializeObject<List<string>>(jObj["imageSM"].ToString());
            var imageSM = _imageSM.Take(param.limit).ToList();
            jObj["imageSM"] = JToken.FromObject(imageSM);

            //imageMD
            List<string> _imageMD = JsonConvert.DeserializeObject<List<string>>(jObj["imageMD"].ToString());
            var imageMD = _imageMD.Take(param.limit).ToList();
            jObj["imageMD"] = JToken.FromObject(imageMD);

            //imageLG
            List<string> _imageLG = JsonConvert.DeserializeObject<List<string>>(jObj["imageLG"].ToString());
            var imageLG = _imageLG.Take(param.limit).ToList();
            jObj["imageLG"] = JToken.FromObject(imageLG);

            response.total = image.Count;
            response.data = jObj.ToObject<object>();

            return Ok(response);
        }

        [HttpGet("legend/{foodId}")]
        public IActionResult Legend([FromQuery] FoodDetailParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "legend.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("ingredients/{foodId}")]
        public IActionResult Ingredient([FromQuery] FoodDetailParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "ingredients.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("recommendations/{foodId}")]
        public IActionResult Recommendation([FromQuery] FoodDetailParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "recommendations.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPost("vote/{foodId}")]
        public IActionResult Vote([FromBody] FoodDetailVoteParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "vote.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("vote/exist/{foodId}")]
        public IActionResult VoteExist([FromQuery] FoodDetailVoteExistParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "vote_exist.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("ingredient/legend/{foodId}/{ingredientId}")]
        public IActionResult IngredientLegend([FromQuery] FoodDetailIngredientLegendParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "ingredient",
               "legend.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("comments/{foodId}")]
        public IActionResult Comments([FromQuery] FoodDetailCommentParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "comments.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [Produces("application/json")]
        [HttpPost("review/{foodId}")]
        public IActionResult Review([FromForm] FoodDetailReviewParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "food",
               "review.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

    }
}
