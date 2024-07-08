using MarketMate.Application.Abstractions.Services;
using MarketMate.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MarketMate.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IPhotoOrganizer, PhotoOrganizer>();

        return services;
    }
}
