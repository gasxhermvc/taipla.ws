using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Restaurant;

namespace Taipla.Webservice.Business.Interfaces.Frontend
{
    public interface IRestaurantService
    {
        Task<BaseResponse> Detail(RestaurantParameter param);

        Task<BaseResponse> Menu(RestaurantParameter param);

        Task<BaseResponse> MenuDetail(RestaurantMenuDetailParameter param);

        Task<BaseResponse> Legend(RestaurantMenuDetailParameter param);

        //Task<BaseResponse> MenuIngredient(RestaurantMenuIngredientParameter param);

        Task<BaseResponse> Vote(RestaurantDetailVoteParameter param);

        Task<BaseResponse> VoteExist(RestaurantDetailParameter param);

        Task<BaseResponse> Comments(RestaurantDetailCommentParameter param);

        Task<BaseResponse> Review(RestaurantDetailReviewParameter param);

        Task<BaseResponse> Promotion(RestaurantPromotionParameter param);
    }
}
