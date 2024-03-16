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
    public class EFFindUsers
    {
        private readonly TAIPLA_DbContext _dbContext;

        public EFFindUsers(TAIPLA_DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<User> GetUsers(List<int> userIds)
        {
            var users = _dbContext.User.Where(w => userIds.Contains(w.UserId)).ToList();

            return users;
        }

        public List<TResult> GetUsers<TResult>(Expression<Func<User, TResult>> select, List<int> userIds) where TResult : class
        {
            var query = _dbContext.User.Where(w => userIds.Contains(w.UserId)).AsQueryable();

            var users = query.Select(select);

            return users.ToList();
        }

        public UserDevice GetDevice(string deviceIdOrAccessToken)
        {
            var query = _dbContext.UserDevice.Where(w => w.DeviceId == deviceIdOrAccessToken || w.Token == deviceIdOrAccessToken)
                .FirstOrDefault();

            return query;
        }
    }
}