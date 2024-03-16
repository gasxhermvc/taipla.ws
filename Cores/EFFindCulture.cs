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
    public class EFFindCulture
    {
        private readonly TAIPLA_DbContext _dbContext;

        public EFFindCulture(TAIPLA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<FoodCulture> GetCultures(List<int> cultureIds)
        {
            var countries = _dbContext.FoodCulture.Where(w => cultureIds.Contains(w.CultureId)).ToList();

            return countries;
        }

        public List<TResult> GetCultures<TResult>(Expression<Func<FoodCulture,TResult>> select,List<int> cultureIds) where TResult : class
        {
            var query = _dbContext.FoodCulture.Where(w => cultureIds.Contains(w.CultureId)).AsQueryable();

            var contries = query.Select(select);

            return contries.ToList(); 
        }
    }
}