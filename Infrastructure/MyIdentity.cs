using System;
using System.Linq;
using System.Security.Claims;

namespace AspNetCoreApiExample.Infrastructure
{
    [Serializable]
    public sealed class MyIdentity : ClaimsIdentity
    {
        public MyIdentity(string userId, string userName)
            : base("Custom", ClaimTypes.Name,
                ClaimTypes.Role)
        {
            AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
            AddClaim(new Claim(ClaimTypes.Name, userName, ClaimValueTypes.Integer32));
        }


        public int Id => Convert.ToInt32(FindAll(ClaimTypes.NameIdentifier).Last().Value);

    }
}