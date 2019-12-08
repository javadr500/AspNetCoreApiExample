using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreApiExample.Domain
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public virtual ICollection<UserToken> UserTokens { get; set; }
        public virtual ICollection<GeoHistory> GeoHistories{ get; set; }

    }
}