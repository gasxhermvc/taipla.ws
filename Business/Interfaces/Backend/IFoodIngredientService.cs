using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.FoodIngredient;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface IFoodIngredientService
    {
        public Task<BaseResponse> FoodIngredients(FoodIngredientFoodIngredientsParameter param);
        public Task<BaseResponse> GetFoodIngredient(FoodIngredientFoodIngredientsParameter param);
        public Task<BaseResponse> Created(FoodIngredientCreatedParameter param);
        public Task<BaseResponse> Updated(FoodIngredientUpdatedParameter param);
        public Task<BaseResponse> Deleted(FoodIngredientDeletedParameter param);
    }
}
