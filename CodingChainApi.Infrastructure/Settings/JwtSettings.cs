namespace CodingChainApi.Infrastructure.Settings
{
    public class JwtSettings : IJwtSettings
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }

        public int? MinutesDuration { get; set; }
    }
}