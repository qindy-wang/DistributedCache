using DistributedCache.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DistributedCache.DI
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            var configuration = BuildConfiguration();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddDistributedSqlServerCache(options=> 
            {
                options.ConnectionString = configuration["SqlServerDistributedCache:ConnectionString"];
                options.SchemaName = configuration["SqlServerDistributedCache:SchemaName"];
                options.TableName = configuration["SqlServerDistributedCache:TableName"];
            });
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = $"{configuration["RedisDistributedCache:IPAddress"]}:{configuration["RedisDistributedCache:Port"]},password={configuration["RedisDistributedCache:Password"]}";
                options.InstanceName = configuration["RedisDistributedCache:InstanceName"];
            });

            services.AddTransient<SqlServerService>();
            services.AddTransient<RedisService>();
            services.AddSingleton(provider =>
            {
                Func<CacheType, IDistributedService> func = type =>
                {
                    switch (type)
                    {
                        case CacheType.SQL:
                            return provider.GetService<SqlServerService>();
                        case CacheType.Redis:
                            return provider.GetService<RedisService>();
                        default:
                            throw new NotSupportedException();
                    }
                };
                return func;
            });
            return services;
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
