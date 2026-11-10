using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceDemo.BuildingBlocks.Infrastructure.Caching;

public static class CachingRegistration
{
    public const string RedisProvider = "Redis";
    public const string MemoryProvider = "Memory";

    public static IServiceCollection AddAppCaching(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var provider = configuration["Cache:Provider"] ?? MemoryProvider;
        if (provider.Equals(RedisProvider, StringComparison.OrdinalIgnoreCase))
        {
            var redis = configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(redis))
            {
                throw new InvalidOperationException("Cache:Provider is Redis but ConnectionStrings:Redis is missing.");
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redis;
                options.InstanceName = "ecommerce-demo:";
            });
            return services;
        }

        if (!provider.Equals(MemoryProvider, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unknown Cache:Provider '{provider}'. Expected '{RedisProvider}' or '{MemoryProvider}'.");
        }

        services.AddDistributedMemoryCache();
        return services;
    }
}
