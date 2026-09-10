namespace Todo.Identity.Constants;

public static class ConfigKeyword
{
    public static class Token
    {
        public const string RefreshTokenExpireInDays = "JwtSettings:RefreshTokenExpireInDays";
        public const string AccessTokenExpireInMinutes = "JwtSettings:AccessTokenExpireInMinutes";
        public const string SecretKey = "JwtSettings:SecretKey";
        public const string Issuer = "JwtSettings:Issuer";
        public const string Audience = "JwtSettings:Audience";
    }
    public static class Appsetting
    {
        public const string BaseUrl = "AppSettings:BaseUrl";
    }
    public static class RabbitMQ
    {
        public const string Host = "RabbitMQ:Host";
        public const string Username = "RabbitMQ:Username";
        public const string Password = "RabbitMQ:Password";
    }
}