using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreApiExample.Caching;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetCoreApiExample.Jwt
{
    public interface ITokenValidatorService
    {
        Task ValidateAsync(TokenValidatedContext context);
    }


    public class TokenValidatorService : ITokenValidatorService
    {
        private readonly IMemoryCache _memoryCache;

        public TokenValidatorService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task ValidateAsync(TokenValidatedContext context)
        {

            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                return;
            }

            var email = claimsIdentity.FindFirst(ClaimTypes.Email).Value;
            if (string.IsNullOrWhiteSpace(email))
            {
                context.Fail("This is not our issued token. It has no user-id.");
                return;
            }

            if (email.ToLower() != "admin@admin.com")
            {
                // user has changed his/her password/roles/stat/IsActive
                context.Fail("This token is expired. Please login again.");
                return;

            }



            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken == null || string.IsNullOrWhiteSpace(accessToken.RawData))
            {
                context.Fail("This token is not in our database.");
                return;
            }

            _memoryCache.TryGetValue(CacheKeys.AccessToken, out string _accessToken);
            if (string.IsNullOrWhiteSpace(_accessToken) || _accessToken != accessToken.RawData) 
            {
                context.Fail("This token is not in our database.");
                return;
            }

        }
    }
}
