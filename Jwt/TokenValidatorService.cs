using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreApiExample.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreApiExample.Jwt
{
    public interface ITokenValidatorService
    {
        Task ValidateAsync(TokenValidatedContext context);
    }


    public class TokenValidatorService : ITokenValidatorService
    {
        private readonly MyDBContext _dbContext;

        public TokenValidatorService(MyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ValidateAsync(TokenValidatedContext context)
        {
            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                return;
            }

            var userId = claimsIdentity.FindAll(ClaimTypes.NameIdentifier).Last().Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                context.Fail("This is not our issued token. It has no user-id.");
                return;
            }


            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken == null || string.IsNullOrWhiteSpace(accessToken.RawData))
            {
                context.Fail("This token is not in our database.");
                return;
            }

            var r = _dbContext.UserTokens.Count();
            var userToken = await
                _dbContext.UserTokens.FirstOrDefaultAsync(x => x.AccessToken == accessToken.RawData &&
                                                               x.OwnerUserId == Int32.Parse(userId));


            if (userToken?.AccessTokenExpiration >= DateTime.UtcNow)
            { }
            else
            {
                context.Fail("This token is not in our database.");
            }
        }
    }
}