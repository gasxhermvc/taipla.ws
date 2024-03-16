using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Food;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface IFoodService
    {
        public Task<BaseResponse> Foods(FoodFoodsParameter param);
        public Task<BaseResponse> GetFood(FoodFoodsParameter param);
        public Task<BaseResponse> Created(FoodCreatedParameter param);
        public Task<BaseResponse> Updated(FoodUpdatedParameter param);
        public Task<BaseResponse> Deleted(FoodDeletedParameter param);


        //=>new
        public Task<BaseResponse> Medias(FoodFoodsParameter param);
    }
}
