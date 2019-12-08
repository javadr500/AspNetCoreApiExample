using System;

namespace AspNetCoreApiExample.Models
{
    public class CredentialsDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public DateTimeOffset AccessTokenExpiration { get; set; }
        public DateTimeOffset RefreshTokenExpiration { get; set; }
    }
}