using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.FirstPage;

namespace Taipla.Webservice.Business.Interfaces.Frontend
{
    public interface IFirstPageService
    {
        Task<BaseResponse> Foods(FirstPageFoodParameter param);

        Task<BaseResponse> FoodTopViewer(FirstPageFoodTopViewerParameter param);

        Task<BaseResponse> Categories(FirstPageFoodCategoriesParameter param);

        Task<BaseResponse> Cultures(FirstPageFoodCulturesParameter param);

        Task<BaseResponse> RestaurantTopViewer(FirstPageRestaurantParameter param);

        Task<BaseResponse> RestaurantPromotions(FirstPageRestaurantParameter param);

        Task<BaseResponse> RestaurantNear(FirstPageRestaurantNearParameter param);
    }
}
