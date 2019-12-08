using Microsoft.Extensions.Configuration;

namespace AspNetCoreApiExample.Jwt
{
    public class JwtConfigModel
    {
        private const string _key = "Jwt:Key";
        private const string _issuer = "Jwt:Issuer";
        private const string _audience = "Jwt:Audience";
        private const string _accessTokenExpirationMinutes = "Jwt:AccessTokenExpirationMinutes";
        private const string _refreshTokenExpirationMinutes = "Jwt:RefreshTokenExpirationMinutes";
        private const string _allowMultipleLoginsFromTheSameUser = "Jwt:AllowMultipleLoginsFromTheSameUser";
        private const string _allowSignoutAllUserActiveClients = "Jwt:AllowSignoutAllUserActiveClients";

        public static string Key;
        public static string Issuer;
        public static string Audience;
        public static double AccessTokenExpirationMinutes;
        public static double RefreshTokenExpirationMinutes;
        public static bool AllowMultipleLoginsFromTheSameUser;
        public static bool AllowSignoutAllUserActiveClients;


        public static void Init(IConfiguration config)
        {
            Key = config[_key];
            Issuer = config[_issuer];
            Audience = config[_audience];


            if (double.TryParse(config[_accessTokenExpirationMinutes], out var expirAtMin) == false)
                expirAtMin = 2;
            AccessTokenExpirationMinutes = expirAtMin;

            if (double.TryParse(config[_refreshTokenExpirationMinutes], out expirAtMin) == false)
                expirAtMin = 60;
            RefreshTokenExpirationMinutes = expirAtMin;

            if (bool.TryParse(config[_allowMultipleLoginsFromTheSameUser], out var enable) == false)
                enable = false;
            AllowMultipleLoginsFromTheSameUser = enable;


            if (bool.TryParse(config[_allowSignoutAllUserActiveClients], out enable) == false)
                enable = false;
            AllowSignoutAllUserActiveClients = enable;
        }
    }
}