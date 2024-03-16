using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Food;

namespace Taipla.Webservice.Business.Interfaces.Frontend
{
    public interface IFoodCenterService
    {
        Task<BaseResponse> Detail(FoodDetailParameter param);

        Task<BaseResponse> Images(FoodDetailParameter param);

        Task<BaseResponse> Legend(FoodDetailParameter param);

        //Task<BaseResponse> Ingredient(FoodDetailParameter param);

        Task<BaseResponse> Recommendation(FoodDetailParameter param);

        Task<BaseResponse> Vote(FoodDetailVoteParameter param);

        Task<BaseResponse> VoteExist(FoodDetailVoteExistParameter param);

        //Task<BaseResponse> IngredientLegend(FoodDetailIngredientLegendParameter param);

        Task<BaseResponse> Comments(FoodDetailCommentParameter param);

        Task<BaseResponse> Review(FoodDetailReviewParameter param);
    }
}
