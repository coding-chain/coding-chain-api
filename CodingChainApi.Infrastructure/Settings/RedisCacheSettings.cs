namespace CodingChainApi.Infrastructure.Settings
{
    public interface IRedisCacheSettings
    {
        string ConnectionString { get; set; }
    }

    public class RedisCacheSettings : IRedisCacheSettings
    {
        public string ConnectionString { get; set; }
    }
}