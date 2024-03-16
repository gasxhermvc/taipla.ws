using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Culture;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface ICultureService
    {
        public Task<BaseResponse> Cultures(CultureCulturesParameter param);
        public Task<BaseResponse> GetCulture(CultureCulturesParameter param);
        public Task<BaseResponse> Created(CultureCreatedParameter param);
        public Task<BaseResponse> Updated(CultureUpdatedParameter param);
        public Task<BaseResponse> Deleted(CultureDeletedParameter param);
    }
}
