using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCoreApiExample.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace AspNetCoreApiExample.Controllers.v1
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [EnableCors("CorsPolicy")]
    [Authorize]
    public class BaseController : ControllerBase
    {

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
