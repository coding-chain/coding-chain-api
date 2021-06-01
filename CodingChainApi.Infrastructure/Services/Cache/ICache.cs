namespace CodingChainApi.Infrastructure.Services.Cache
{
    public interface ICache
    {
        bool SetCache<T>(T values, object key, int durationSeconds);
        T? GetCache<T>(object key) where T : class;
        bool RemoveCache(object key);
    }
}