using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Extensions;
using Taipla.Webservice.Models.Parameters.Backend.FoodIngredient;

namespace Taipla.Webservice.Controllers.v1.Backend
{
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/backend/[controller]")]
    [ApiController]
    public class FoodIngredientController : ControllerBase
    {
        private readonly IFoodIngredientService _foodIngredient;

        public FoodIngredientController(IFoodIngredientService foodIngredient)
        {
            DateTimeExtension.SetDateEnv();

            _foodIngredient = foodIngredient;
        }

        [HttpGet("FoodIngredients")]
        public async Task<IActionResult> FoodIngredients([FromQuery] FoodIngredientFoodIngredientsParameter param)
        {
            var response = await _foodIngredient.FoodIngredients(param);

            return Ok(response);
        }

        [HttpGet("FoodIngredients/{COUNTRY_ID}")]
        public async Task<IActionResult> GetFoodIngredient([FromQuery] FoodIngredientFoodIngredientsParameter param)
        {
            var response = await _foodIngredient.GetFoodIngredient(param);

            return Ok(response);
        }

        [HttpPost("created")]
        public async Task<IActionResult> Created([FromBody] FoodIngredientCreatedParameter param)
        {
            var response = await _foodIngredient.Created(param);

            return Ok(response);
        }

        [HttpPut("updated")]
        public async Task<IActionResult> Updated([FromBody] FoodIngredientUpdatedParameter param)
        {
            var response = await _foodIngredient.Updated(param);

            return Ok(response);
        }

        [HttpDelete("deleted")]
        public async Task<IActionResult> Deleted([FromQuery] FoodIngredientDeletedParameter param)
        {
            var response = await _foodIngredient.Deleted(param);

            return Ok(response);
        }
    }
}
