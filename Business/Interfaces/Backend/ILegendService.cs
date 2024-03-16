using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Legend;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface ILegendService
    {
        public Task<BaseResponse> Legends(LegendLegendsParameter param);
        public Task<BaseResponse> GetLegend(LegendLegendsParameter param);
        public Task<BaseResponse> Created(LegendCreatedParameter param);
        public Task<BaseResponse> Updated(LegendUpdatedParameter param);
        public Task<BaseResponse> Deleted(LegendDeletedParameter param);
    }
}
