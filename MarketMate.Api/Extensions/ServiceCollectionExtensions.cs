using MarketMate.Api.Filters;

namespace MarketMate.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<WithVkAccessTokenFilter>();
        services.AddProblemDetails();
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddHttpClient();
        services.AddHttpContextAccessor();

        return services;
    }
}
