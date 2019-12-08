using System;
using System.Security.Claims;
using AspNetCoreApiExample.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreApiExample.Controllers.v1
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [EnableCors("CorsPolicy")]
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected int CurrentUserId
        {
            get
            {
                var claims = User.Identity as MyIdentity;
                if (claims != null)
                {
                    return claims.Id;
                }

                return 0;
            }
        }


        protected AppResult SuccessfullResult(object data = null)
        {
            var result = new AppResult { Success = true, Data = data };
            return result;
        }


        protected AppResult SuccessfullMessage(object data = null)
        {
            var result = new AppResult
            {
                Success = true,
                Data = data
            };

            return result;
        }

        protected AppResult ErrorMessage(string message, object data = null)
        {
            var result = new AppResult
            {
                Success = false,
                Data = data,
                Description = message
            };

            return result;
        }
    }
}