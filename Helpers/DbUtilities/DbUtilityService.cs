using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Helpers.DbUtilities
{
    public class DbUtilityService : IDbUtilityService
    {
        private DbContext Context { get; set; }

        public TransactionResult CreateTransaction(Action action)
        {
            bool success = false;
            TransactionResult result = new TransactionResult()
            {
                success = success,
                exception = null
            };

            try
            {
                this.Context.Database.BeginTransaction();
                action();
                this.Context.Database.CommitTransaction();
                success = true;
                result.success = true;
            }
            catch (Exception e)
            {
                this.Context.Database.RollbackTransaction();
                result.success = false;
                result.exception = e;
            }

            return result;
        }

        public void SetDbContext(DbContext dbContext)
        {
            this.Context = dbContext;
        }
    }
}
