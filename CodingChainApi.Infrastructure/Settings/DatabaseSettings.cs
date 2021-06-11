namespace CodingChainApi.Infrastructure.Settings
{
    public interface IDatabaseSettings
    {
        string? ConnectionString { get; set; }
    }
    public class DatabaseSettings : IDatabaseSettings
    {
        public string? ConnectionString { get; set; }
    }
}