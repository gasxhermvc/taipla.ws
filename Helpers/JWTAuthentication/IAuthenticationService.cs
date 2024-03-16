using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Taipla.Webservice.Helpers.JWTAuthentication
{
    public abstract class IAuthenticationService<TUser>
          where TUser : class
    {
        public abstract Dictionary<string, dynamic> GenerateToken(List<Claim> claims, bool rememberMe);
        public abstract ClaimsPrincipal UserContext();
        public abstract bool IsAuthenticated { get; }
        public abstract TUser User { get; }
    }
}
