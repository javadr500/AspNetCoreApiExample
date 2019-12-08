using System;

namespace AspNetCoreApiExample.Models
{
    public class CredentialsDto
    {
        public string AccessToken { get; set; }
        public DateTimeOffset AccessTokenExpiration { get; set; }
    }
}