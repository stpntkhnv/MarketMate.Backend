using MarketMate.Utilities.Helpers;
using MarketMate.Utilities.Helpers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MarketMate.Utilities.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUtilityServices(this IServiceCollection services)
    {
        services.AddScoped<IUserContextAccessor, UserContextAccessor>();

        return services;
    }
}
