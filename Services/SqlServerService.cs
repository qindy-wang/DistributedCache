using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DistributedCache.Services
{
    public class SqlServerService: ISqlServerService
    {
        private readonly IDistributedCache _cacheService;

        public SqlServerService(IDistributedCache cacheService)
        {
            this._cacheService = cacheService;
        }

        public async Task SetAsync(string key, byte[] value, object expiration = null, bool isAbsoluteExpiration = false)
        {
            var options = this.BuildDistributedCacheEntryOptions(expiration, isAbsoluteExpiration);
            await _cacheService.SetAsync(key, value, options);
        }

        public async Task SetAsync(string key, string value, object expiration = null, bool isAbsoluteExpiration = false)
        {
            var options = this.BuildDistributedCacheEntryOptions(expiration, isAbsoluteExpiration);
            await _cacheService.SetStringAsync(key, value, options);
        }

        public async Task<byte[]> GetAsync(string key)
        {
            return await _cacheService.GetAsync(key);
        }

        public async Task<string> GetStringAsync(string key)
        {
            return await _cacheService.GetStringAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _cacheService.RemoveAsync(key);
        }

        public async Task RefreshAsync(string key)
        {
            await _cacheService.RefreshAsync(key);
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
