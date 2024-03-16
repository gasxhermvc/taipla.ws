using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Log;

namespace Taipla.Webservice.Business.Interfaces.Frontend
{
    public interface ILogService
    {
        Task<BaseResponse> Restaurant(LogRestaurantParameter param);
        Task<BaseResponse> RestaurantMenu(LogRestaurantMenuParameter param);
        Task<BaseResponse> RestaurantPromotion(LogRestaurantPromotionParameter param);
        Task<BaseResponse> Food(LogFoodParameter param);
    }
}
