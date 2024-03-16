using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Helpers.DbUtilities
{
    public interface IDbUtilityService
    {
        public TransactionResult CreateTransaction(Action action);

        public void SetDbContext(DbContext dbContext);
    }
}
