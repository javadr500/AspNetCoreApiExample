using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreApiExample.Domain;
using AspNetCoreApiExample.Infrastructure;
using AspNetCoreApiExample.Jwt;
using AspNetCoreApiExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreApiExample.Controllers.v1
{
    public class AccountController : BaseController
    {
        private readonly MyDBContext _dbContext;

        public AccountController(
            MyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<AppResult> Register([FromBody] LoginDto model)
        {
            if (model == null ||
                string.IsNullOrWhiteSpace(model.UserName) ||
                string.IsNullOrWhiteSpace(model.Password))
                return ErrorMessage("Error");


            var u = await _dbContext.Users.SingleOrDefaultAsync(x =>
                x.UserName == model.UserName &&
                x.Password == model.Password);
            if (u != null)
                return ErrorMessage("user  exist");

            var dbUser = new User
            {
                UserName = model.UserName,
                Password = model.Password
            };

            _dbContext.Users.Add(dbUser);
            await _dbContext.SaveChangesAsync();

            var data = await BuidlCredentials(model, dbUser.UserId);
            return SuccessfullMessage(data);
        }


        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<AppResult> Login([FromBody] LoginDto login)
        {
            if (login == null ||
                string.IsNullOrWhiteSpace(login.UserName) ||
                string.IsNullOrWhiteSpace(login.Password))
                return ErrorMessage("error . enter username or password ");

            var r = await _dbContext.Users.SingleOrDefaultAsync(
                x => x.UserName == login.UserName &&
                     x.Password == login.Password);
            if (r == null)
                return ErrorMessage("user not exist");

            var data = await BuidlCredentials(login, r.UserId);
            return SuccessfullMessage(data);
        }

        private async Task<CredentialsDto> BuidlCredentials(LoginDto login, int userId)
        {
            var now = DateTimeOffset.UtcNow;
            var accessToken = await BuildToken(login.UserName, userId);
            var accessTokenExpiration = now.AddMinutes(JwtConfigModel.AccessTokenExpirationMinutes);

            var token = new UserToken
            {
                UserTokenGuid = Guid.NewGuid(),
                OwnerUserId = userId,
                AccessToken = accessToken,
                AccessTokenExpiration = accessTokenExpiration
            };
            _dbContext.UserTokens.Add(token);
            await _dbContext.SaveChangesAsync();
            var r = _dbContext.UserTokens.Count();
            var data = new CredentialsDto
            {
                AccessToken = accessToken,
                AccessTokenExpiration = accessTokenExpiration
            };

            return data;
        }

        private async Task<string> BuildToken(string userName, int userId)
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

                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Sid, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userName)
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