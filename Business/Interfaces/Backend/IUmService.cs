using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Um;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface IUmService
    {
        public Task<BaseResponse> Users(UmUserParameter param);
        public Task<BaseResponse> GetUser(UmUserParameter param);
        public Task<BaseResponse> Created(UmCreatedParameter param);
        public Task<BaseResponse> Updated(UmUpdatedParameter param);
        public Task<BaseResponse> Deleted(UmDeletedParameter param);
        public Task<BaseResponse> ChangePassword(UmChangePasswordParameter param);
    }
}
