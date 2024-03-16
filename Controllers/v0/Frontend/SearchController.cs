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
using Taipla.Webservice.Models.Parameters.Frontend.Search;

namespace Taipla.Webservice.Controllers.v0.Frontend
{
    [ApiVersion("0")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IWebHostEnvironment _web;

        public SearchController(IWebHostEnvironment web)
        {
            _web = web;
        }

        [HttpGet("histories/{deviceId}")]
        public IActionResult Histories([FromRoute] SearchHistoryParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "search",
               "histories.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Search([FromBody] SearchParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "search",
               "search.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPut("history/remove/{searchId}/{deviceId}")]
        public IActionResult HistoryRemove([FromRoute] SearchHistoryRemoveParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "search",
               "history",
               "remove.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpPut("history/remove/all/{deviceId}")]
        public IActionResult HistoryRemoveAll([FromRoute] SearchHistoryRemoveAllParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "search",
               "history",
               "remove_all.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }

        [HttpGet("results")]
        public IActionResult Results([FromQuery] SearchParameter param)
        {
            var jsonFile = System.IO.Path.Combine(
               PathExtension.BasePath(_web),
               "mockup",
               "frontend",
               "search",
               "results.200.json");

            var json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);

            var response = JsonConvert.DeserializeObject<BaseResponse>(json);

            return Ok(response);
        }
    }
}
