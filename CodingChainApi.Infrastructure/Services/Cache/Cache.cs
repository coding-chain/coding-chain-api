using System;
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

        public T? GetCache<T>(object key) where T : class
        {
            try
            {
                return _cache.Get<T>(key);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
            }

            return null;
        }


        public bool SetCache<T>(T values, object key, int durationSeconds = 120)
        {
            try
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(durationSeconds));
                _cache.Set(key, values, cacheEntryOptions);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
            }

            return false;
        }


        public bool RemoveCache(object key)
        {
            try
            {
                _cache.Remove(key);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("{Message}", ex.Message);
            }

            return false;
        }
    }
}