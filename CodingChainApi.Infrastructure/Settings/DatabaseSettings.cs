namespace CodingChainApi.Infrastructure.Settings
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string? ConnectionString { get; set; }
    }
}