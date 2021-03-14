using DistributedCache.DI;
using DistributedCache.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DistributedCache
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();
            var cacheService = (RedisService)serviceProvider.GetService(typeof(RedisService));

            cacheService.SetAsync("key01", "value01").GetAwaiter().GetResult();
            var cacheValue = cacheService.GetStringAsync("key01").GetAwaiter().GetResult();
    
            Console.ReadKey();
        }
    }
}
