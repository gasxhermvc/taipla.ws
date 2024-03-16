using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Attributes;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [ApiController]
    public class LUTController : ControllerBase
    {
        private readonly ILUTService _lut;

        public LUTController(ILUTService lut)
        {
            DateTimeExtension.SetDateEnv();

            _lut = lut;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> Countries([FromQuery, ValueFilter] int? COUNTRY_ID)
        {
            var response = await _lut.Countries(COUNTRY_ID);

            return Ok(response);
        }

        [HttpGet("cultures")]
        public async Task<IActionResult> Cultures([FromQuery, ValueFilter] int? COUNTRY_ID,
            [FromQuery, ValueFilter] int? CULTURE_ID)
        {
            var response = await _lut.Cultures(COUNTRY_ID, CULTURE_ID);

            return Ok(response);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> Roles()
        {
            var response = await _lut.Roles();

            return Ok(response);
        }

        [HttpGet("legend-types")]
        public async Task<IActionResult> LegendTypes([FromQuery, ValueFilter] int? LEGEND_TYPE)
        {
            var response = await _lut.LegendTypes(LEGEND_TYPE);

            return Ok(response);
        }

        [HttpGet("owners")]
        public async Task<IActionResult> Owners()
        {
            var response = await _lut.Owners();

            return Ok(response);
        }

        [HttpGet("staff")]
        public async Task<IActionResult> Staff([FromQuery, ValueFilter] int? PARENT_ID)
        {
            var response = await _lut.Staff(PARENT_ID);

            return Ok(response);
        }

        [HttpGet("promotion-types")]
        public async Task<IActionResult> PromotionTypes()
        {
            var response = await _lut.PromotionTypes();

            return Ok(response);
        }

        [HttpGet("author-create-restaurant")]
        public async Task<IActionResult> AuthorCreateRestaurant()
        {
            var response = await _lut.AuthorCreateRestaurant();

            return Ok(response);
        }

        [HttpGet("author-create-food-center")]
        public async Task<IActionResult> AuthorCreateFoodCenter()
        {
            var response = await _lut.AuthorCreateFoodCenter();

            return Ok(response);
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> Provinces()
        {
            var response = await _lut.Provinces();

            return Ok(response);
        }
    }
}
