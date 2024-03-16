using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Interfaces.Backend;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.FoodIngredient;

namespace Taipla.Webservice.Business.Services.Backend
{
    public class FoodIngredientService : IFoodIngredientService
    {
        public Task<BaseResponse> FoodIngredients(FoodIngredientFoodIngredientsParameter param)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> GetFoodIngredient(FoodIngredientFoodIngredientsParameter param)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> Created(FoodIngredientCreatedParameter param)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> Updated(FoodIngredientUpdatedParameter param)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> Deleted(FoodIngredientDeletedParameter param)
        {
            throw new NotImplementedException();
        }
    }
}
