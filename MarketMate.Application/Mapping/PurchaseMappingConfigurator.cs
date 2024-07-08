using Mapster;
using MarketMate.Application.Dtos;
using MarketMate.Domain.Entities;

namespace MarketMate.Application.Mapping;

public static class PurchaseMappingConfigurator
{
    public static void ConfigureMappings()
    {
        TypeAdapterConfig<PurchaseDto, Purchase>.NewConfig();
    }
}
