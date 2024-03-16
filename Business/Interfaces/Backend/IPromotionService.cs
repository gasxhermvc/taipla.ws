using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Promotion;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface IPromotionService
    {
        public Task<BaseResponse> Promotions(PromotionPromotionsParameter param);
        public Task<BaseResponse> GetPromotion(PromotionPromotionsParameter param);
        public Task<BaseResponse> Created(PromotionCreatedParameter param);
        public Task<BaseResponse> Updated(PromotionUpdatedParameter param);
        public Task<BaseResponse> Deleted(PromotionDeletedParameter param);
    }
}
