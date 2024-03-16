using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.Country;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _country;

        public CountryController(ICountryService country)
        {
            DateTimeExtension.SetDateEnv();

            _country = country;
        }

        [HttpGet("countries")]
        public async Task<IActionResult> Countries([FromQuery] CountryCountriesParameter param)
        {
            var response = await _country.Countries(param);

            return Ok(response);
        }


        [HttpGet("country/{COUNTRY_ID}")]
        public async Task<IActionResult> GetCountry([FromQuery]CountryCountriesParameter param)
        {
            var response = await _country.GetCountry(param);

            return Ok(response);
        }

        [HttpPost("created")]
        public async Task<IActionResult> Created([FromForm] CountryCreatedParameter param)
        {
            var response = await _country.Created(param);

            return Ok(response);
        }

        [HttpPut("updated")]
        public async Task<IActionResult> Updated([FromForm] CountryUpdatedParameter param)
        {
            var response = await _country.Updated(param);

            return Ok(response);
        }

        [HttpDelete("deleted")]
        public async Task<IActionResult> Deleted([FromQuery] CountryDeletedParameter param)
        {
            var response = await _country.Deleted(param);

            return Ok(response);
        }
    }
}
