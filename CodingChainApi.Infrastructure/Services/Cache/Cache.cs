using System;
using System.Threading.Tasks;
using CodingChainApi.Infrastructure.Common.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CodingChainApi.Infrastructure.Services.Cache
{
    public class Cache : ICache
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<Cache> _logger;


        public Cache(ILogger<Cache> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public Task<T?> GetCache<T>(object key) where T : class
        {
            try
            {
                var result = _cache.Get<T?>(key);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
            }

            return Task.FromResult<T?>(null);
        }


        public Task<bool> SetCache<T>(T values, object key, int durationSeconds)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(durationSeconds));
                _cache.Set(key, values, cacheEntryOptions);
                return true.ToTask();
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
            }

            return false.ToTask();
        }


        public Task<bool> RemoveCache(object key)
        {
            try
            {
                _cache.Remove(key);
                return true.ToTask();
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
            }

            return false.ToTask();
        }
    }
}