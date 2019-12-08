using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace AspNetCoreApiExample.Infrastructure
{
    public class AppResult
    {
        public object Data { get; set; }
        public bool Success { get; set; }
        public string Description { get; set; }




    }

}
