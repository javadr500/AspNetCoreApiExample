using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreApiExample.Domain
{
    public class UserToken
    {
        [Key]
        public Guid UserTokenGuid { get; set; }
        public int OwnerUserId { get; set; }
        public string AccessToken { get; set; }
        public DateTimeOffset AccessTokenExpiration { get; set; }

        public virtual User User { get; set; }
    }
}