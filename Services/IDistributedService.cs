using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DistributedCache.Services
{
    public interface IDistributedService
    {
        Task SetAsync(string key, byte[] value, object expiration = null, bool isAbsoluteExpiration = false);

        Task SetAsync(string key, object value, object expiration = null, bool isAbsoluteExpiration = false);

        Task<T> GetAsync<T>(string key);

        Task<byte[]> GetAsync(string key);

        Task<string> GetStringAsync(string key);

        Task RemoveAsync(string key);

        Task RefreshAsync(string key);
    }

    public enum CacheType
    {
        SQL = 0,
        Redis = 1
    }
}
