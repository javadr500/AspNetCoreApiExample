using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreApiExample.Caching
{
    public static class CacheKeys
    {
       
        public static string AccessToken { get { return "_AccessToken"; } }
        public static string Geo { get { return "_Geo"; } }
    }
}
