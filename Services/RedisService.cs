using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DistributedCache.Services
{
    public class RedisService : IDistributedService
    {
        private readonly IDistributedCache _cacheService;

        public RedisService(IDistributedCache cacheService)
        {
            this._cacheService = cacheService;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            T obj = default(T);
            var value = await _cacheService.GetStringAsync(key);
            if (string.IsNullOrEmpty(value))
            { 
                return default(T);
            }
            try
            {
                obj = JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(ex.Message, ex);
            }
            return obj;
        }

        public async Task<byte[]> GetAsync(string key)
        {
            return await _cacheService.GetAsync(key);
        }

        public async Task<string> GetStringAsync(string key)
        {
            return await _cacheService.GetStringAsync(key);
        }

        public async Task RefreshAsync(string key)
        {
            await _cacheService.RefreshAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _cacheService.RemoveAsync(key);
        }

        public async Task SetAsync(string key, object value, object expiration = null, bool isAbsoluteExpiration = false)
        {
            var jsonValue = string.Empty;
            try
            {
                jsonValue = JsonConvert.SerializeObject(value);
                var options = this.BuildDistributedCacheEntryOptions(expiration, isAbsoluteExpiration);
                await _cacheService.SetStringAsync(key, jsonValue, options);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(ex.Message, ex);
            }
        }

        public async Task SetAsync(string key, byte[] value, object expiration = null, bool isAbsoluteExpiration = false)
        {
            var options = this.BuildDistributedCacheEntryOptions(expiration, isAbsoluteExpiration);
            await _cacheService.SetAsync(key, value, options);
        }

        private DistributedCacheEntryOptions BuildDistributedCacheEntryOptions(object expiration = null, bool isAbsoluteExpiration = false)
        {
            var options = new DistributedCacheEntryOptions();
            if (expiration != null)
            {
                if (expiration is TimeSpan)
                {
                    if (isAbsoluteExpiration)
                        options.SetAbsoluteExpiration((TimeSpan)expiration);
                    else
                        options.SetSlidingExpiration((TimeSpan)expiration);
                }
                else if (expiration is DateTimeOffset)
                {
                    options.SetAbsoluteExpiration((DateTimeOffset)expiration);
                }
                else
                {
                    throw new NotSupportedException("Not support current expiration object settings.");
                }
            }
            return options;
        }
    }
}
