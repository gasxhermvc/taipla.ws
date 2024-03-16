using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Backend.Auth;

namespace Taipla.Webservice.Business.Interfaces.Backend
{
    public interface IAuthService
    {
        public Task<BaseResponse> Login(LoginParameter param);

        public Task<BaseResponse> Logout(string CLIENT_ID);

        public Task<BaseResponse> UserInfo();
    }
}
