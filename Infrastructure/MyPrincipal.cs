using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AspNetCoreApiExample.Infrastructure
{
    public class MyPrincipal : ClaimsPrincipal
    {
        public MyPrincipal(MyIdentity identity)
            : base(identity)
        {
        }

        public static ClaimsPrincipal GeneratePrincipal(string userName, int userID, bool twoFactorAuthIsPassed)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Sid, userID.ToString()),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(identity);
        }
    }


    public class ClaimsTransformer : IClaimsTransformation
    {
    
        public ClaimsTransformer()
        {
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal is MyPrincipal myPrincipal)
            {
                return principal;
            }

            var userName = principal.FindAll(ClaimTypes.Name).Last().Value;
            var userId = principal.FindAll(ClaimTypes.NameIdentifier).Last().Value;
            
            var claimsIdentity = new MyIdentity(userId,userName);
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}