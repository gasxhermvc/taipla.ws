using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Frontend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Frontend.Search;

namespace Taipla.Webservice.Controllers.v1.Frontend
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/frontend/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            DateTimeExtension.SetDateEnv();
            _searchService = searchService;
        }

        [HttpGet("histories/{deviceId}")]
        public async Task<IActionResult> Histories([FromRoute] SearchHistoryParameter param)
        {
            var response = await _searchService.Histories(param);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Search([FromBody] SearchParameter param)
        {
            var response = await _searchService.Search(param);

            return Ok(response);
        }

        [HttpPut("history/remove/{searchId}/{deviceId}")]
        public async Task<IActionResult> HistoryRemove([FromRoute] SearchHistoryRemoveParameter param)
        {
            var response = await _searchService.HistoryRemove(param);

            return Ok(response);
        }

        [HttpPut("history/remove/all/{deviceId}")]
        public async Task<IActionResult> HistoryRemoveAll([FromRoute] SearchHistoryRemoveAllParameter param)
        {
            var response = await _searchService.HistoryRemoveAll(param);

            return Ok(response);
        }

        [HttpPost("results")]
        public async Task<IActionResult> Results([FromBody] SearchParameter param)
        {
            var response = await _searchService.Results(param);

            return Ok(response);
        }
    }
}
