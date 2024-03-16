using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Country;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface ICountryService
    {
        public Task<BaseResponse> Countries(CountryCountriesParameter param);
        public Task<BaseResponse> GetCountry(CountryCountriesParameter param);
        public Task<BaseResponse> Created(CountryCreatedParameter param);
        public Task<BaseResponse> Updated(CountryUpdatedParameter param);
        public Task<BaseResponse> Deleted(CountryDeletedParameter param);
    }
}
