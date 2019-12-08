namespace AspNetCoreApiExample.Infrastructure
{
    public class AccountHelper
    {
        public const string DefaultUserName = "admin@admin.com";
        public const string DefaultPassword = "admin";

        public static bool InvalidUser(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password))
                return false;

            if (userName.ToLower() == DefaultUserName &&
                password.ToLower() == DefaultPassword)
                return true;
            return false;
        }
    }
}