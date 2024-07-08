namespace MarketMate.Application.Mapping;

public static class MappingConfigurator
{
    public static void RegisterMappings()
    {
        PurchaseMappingConfigurator.ConfigureMappings();
    }
}
