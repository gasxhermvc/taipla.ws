using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Auth;

namespace Taipla.Webservice.Business.Interfaces.Frontend
{
    public interface IAuthService
    {
        public Task<BaseResponse> Login(LoginParameter param);

        public Task<BaseResponse> Logout(LogoutParameter param);

        public Task<BaseResponse> UserInfo();
    }
}
