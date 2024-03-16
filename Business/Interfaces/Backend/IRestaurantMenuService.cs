using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.RestaurantMenu;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface IRestaurantMenuService
    {
        public Task<BaseResponse> RestaurantMenus(RestaurantMenuRestaurantMenusParameter param);
        public Task<BaseResponse> GetRestaurantMenu(RestaurantMenuRestaurantMenusParameter param);
        public Task<BaseResponse> Created(RestaurantMenuCreatedParameter param);
        public Task<BaseResponse> Updated(RestaurantMenuUpdatedParameter param);
        public Task<BaseResponse> Deleted(RestaurantMenuDeletedParameter param);


        //=>new
        public Task<BaseResponse> Medias(RestaurantMenuRestaurantMenusParameter param);
    }
}
