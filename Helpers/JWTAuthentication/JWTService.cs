using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Taipla.Webservice.Helpers.JWTAuthentication
{

    public class JWTService<TUser> : IAuthenticationService<TUser>
        where TUser : class
    {
        protected IConfiguration _configuration { get; set; }

        protected IHttpContextAccessor _context { get; set; }

        public override bool IsAuthenticated { get => this.Authentication(); }

        public override TUser User { get; }

        public JWTService(IConfiguration configuration, IHttpContextAccessor context)
        {
            _configuration = configuration;
            _context = context;
        }

        public override Dictionary<string, dynamic> GenerateToken(List<Claim> claims, bool rememberMe)
        {
            //=>default = 1 day
            var timeout = int.Parse(_configuration.GetSection("JWT:TimeOut")?.Value ?? "86400");

            if (rememberMe)
            {
                //=>1 year
                timeout = 31536000;
            }

            DateTime expired = DateTime.Now.AddSeconds(timeout);

            SymmetricSecurityKey symmetricKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:SecretKey").Value));

            SigningCredentials creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = _configuration.GetSection("JWT:SecretKey").Value,
                Expires = expired,
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return new Dictionary<string, dynamic>{
                { "token", tokenHandler.WriteToken(token) },
                { "expired", expired }
            };
        }

        public override ClaimsPrincipal UserContext()
        {
            return _context.HttpContext.User;
        }

        private bool Authentication()
        {
            var userContext = this.UserContext();

            if (userContext.Claims != null && userContext.Claims.Count() > 0)
            {
                var identifier = userContext.Claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier);

                var username = userContext.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name).Value;

                return identifier != null && !string.IsNullOrEmpty(username);
            }

            return false;
        }
    }
}
