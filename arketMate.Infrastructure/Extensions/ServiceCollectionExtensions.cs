using MarketMate.Application.Abstractions.HttpClients;
using MarketMate.Infrastructure.Consts;
using MarketMate.Infrastructure.HttpClients.Handlers;
using MarketMate.Infrastructure.HttpClients.VkMethods;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketMate.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(HttpClientNames.Vk, client =>
        {
            var vkMethodsApiBaseUrl = configuration.GetValue<string>("VkApiSettings:BaseUrl");
            client.BaseAddress = new Uri(vkMethodsApiBaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
            .ConfigurePrimaryHttpMessageHandler(() => new RateLimitingHandler(new HttpClientHandler(), 5));


        services.AddTransient<IVkMethodsApiClient, VkMethodsApiClient>(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            return new VkMethodsApiClient(clientFactory.CreateClient(HttpClientNames.Vk));
        });

        return services;
    }
}
