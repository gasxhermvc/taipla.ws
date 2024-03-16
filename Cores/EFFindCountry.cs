using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Taipla.Webservice.Entities;

namespace Taipla.Webservice.Cores
{
    public class EFFindCountry
    {
        private readonly TAIPLA_DbContext _dbContext;

        public EFFindCountry(TAIPLA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<FoodCountry> GetCountries(List<int> countryIds)
        {
            var countries = _dbContext.FoodCountry.Where(w => countryIds.Contains(w.CountryId)).ToList();

            return countries;
        }

        public List<TResult> GetCountries<TResult>(Expression<Func<FoodCountry,TResult>> select,List<int> countryIds) where TResult : class
        {
            var query = _dbContext.FoodCountry.Where(w => countryIds.Contains(w.CountryId)).AsQueryable();

            var contries = query.Select(select);

            return contries.ToList(); 
        }
    }
}