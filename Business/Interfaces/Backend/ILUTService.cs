using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Business.Enums;
using Taipla.Webservice.Models.Bases;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface ILUTService
    {
        public Task<BaseResponse> Countries(int? COUNTRY_ID);

        public Task<BaseResponse> Cultures(int? COUNTRY_ID, int? CULTURE_ID);

        public Task<BaseResponse> Roles();

        public Task<BaseResponse> LegendTypes(int? LEGEND_TYPE);

        public Task<BaseResponse> Owners();

        public Task<BaseResponse> Staff(int? PARENT_ID);

        public Task<BaseResponse> PromotionTypes();

        public Task<BaseResponse> AuthorCreateRestaurant();

        public Task<BaseResponse> AuthorCreateFoodCenter();


        public Task<BaseResponse> Provinces();
    }
}
