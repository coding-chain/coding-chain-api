using System.Threading.Tasks;

namespace CodingChainApi.Infrastructure.Services.Cache
{
    public interface ICache
    {
        Task<bool> SetCache<T>(T values, object key, int durationSeconds);
        Task<T?> GetCache<T>(object key) where T : class;
        Task<bool> RemoveCache(object key);
    }
}