using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreApiExample.Caching;
using AspNetCoreApiExample.Infrastructure;
using AspNetCoreApiExample.Jwt;
using AspNetCoreApiExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreApiExample.Controllers.v1
{
    public class AccountController : BaseController
    {
        private readonly IMemoryCache _memoryCache;

        public AccountController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<AppResult> Login([FromBody] LoginDto login)
        {

            if (login == null ||
                string.IsNullOrWhiteSpace(login.UserName) ||
                string.IsNullOrWhiteSpace(login.Password))
            {
                return ErrorMessage("error . enter username or password ");
            }

            var r = AccountHelper.InvalidUser(login.UserName, login.Password);
            if (r == false)
                return ErrorMessage("user not exist");

            var now = DateTimeOffset.UtcNow;
            var userTokenGuid = Guid.NewGuid();
            var accessToken = await BuildToken(login.UserName, userTokenGuid);
            var refreshToken = Guid.NewGuid().ToString().Replace("-", "");
            var refreshTokenExpiration = now.AddMinutes(JwtConfigModel.RefreshTokenExpirationMinutes);
            var accessTokenExpiration = now.AddMinutes(JwtConfigModel.AccessTokenExpirationMinutes);


            var data = new CredentialsDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration,
                AccessTokenExpiration = accessTokenExpiration
            };
            _memoryCache.Set(CacheKeys.AccessToken, accessToken);

            return SuccessfullMessage(data);
        }





        private async Task<string> BuildToken(string userName, Guid userTokenGuid)
        {
            var id = Guid.NewGuid();
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, JwtConfigModel.Issuer),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userName),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfigModel.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddMinutes(JwtConfigModel.AccessTokenExpirationMinutes);
            var token = new JwtSecurityToken(
                JwtConfigModel.Issuer,
                JwtConfigModel.Audience,
                claims,
                now,
                expires,
                creds);
            IdentityModelEventSource.ShowPII = true;
            return new JwtSecurityTokenHandler().WriteToken(token);
        }




    }
}