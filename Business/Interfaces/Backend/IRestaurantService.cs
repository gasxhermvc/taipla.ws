using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Restaurant;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface IRestaurantService
    {
        public Task<BaseResponse> Restaurants(RestaurantRestaurantsParameter param);
        public Task<BaseResponse> GetRestaurant(RestaurantRestaurantsParameter param);
        public Task<BaseResponse> Created(RestaurantCreatedParameter param);
        public Task<BaseResponse> Updated(RestaurantUpdatedParameter param);
        public Task<BaseResponse> Deleted(RestaurantDeletedParameter param);

        //=>new
        public Task<BaseResponse> Medias(RestaurantRestaurantsParameter param);
    }
}
