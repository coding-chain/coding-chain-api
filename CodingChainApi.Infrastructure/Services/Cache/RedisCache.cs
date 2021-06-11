using System;
using System.Threading.Tasks;
using Castle.Core.Logging;
using CodingChainApi.Infrastructure.Common.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CodingChainApi.Infrastructure.Services.Cache
{
    public class RedisCache : ICache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RedisCache> _logger;

        public RedisCache(IDistributedCache distributedCache, ILogger<RedisCache> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<bool> SetCache<T>(T values, object key, int durationSeconds)
        {
            try
            {
                await _distributedCache.SetStringAsync(key.ToString(), JsonConvert.SerializeObject(values));
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot set cache {Error}", e.Message);
            }

            return false;
        }

        public async Task<T?> GetCache<T>(object key) where T : class
        {
            try
            {
                var result = await _distributedCache.GetStringAsync(key.ToString());
                return result is null ? null : JsonConvert.DeserializeObject<T?>(result);
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot set cache {Error}", e.Message);
            }

            return null;
        }

        public async Task<bool> RemoveCache(object key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key.ToString());
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