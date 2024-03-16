using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Models.Bases;
using Taipla.Webservice.Models.Parameters.Frontend.Account;

namespace Taipla.Webservice.Business.Interfaces.Frontend
{
    public interface IAccountService
    {
        Task<BaseResponse> Register(RegisterParameter param);
        Task<BaseResponse> Profile();
        Task<BaseResponse> UpdateProfile(AccountProfileParameter param);
        Task<BaseResponse> ChangePassword(ChangePasswordParameter param);
    }
}
