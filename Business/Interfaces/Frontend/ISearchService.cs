using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Search;

namespace Taipla.Webservice.Business.Interfaces.Frontend
{
    public interface ISearchService
    {
        Task<BaseResponse> Histories(SearchHistoryParameter param);
        Task<BaseResponse> Search(SearchParameter param);
        Task<BaseResponse> HistoryRemove(SearchHistoryRemoveParameter param);
        Task<BaseResponse> HistoryRemoveAll(SearchHistoryRemoveAllParameter param);
        Task<BaseResponse> Results(SearchParameter param);
    }
}
