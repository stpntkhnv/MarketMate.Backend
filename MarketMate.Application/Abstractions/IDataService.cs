namespace MarketMate.Application.Abstractions;

public interface IDataService
{
    Task SaveShopDataAsync(string shopInfo, string lineAndPlace);
    Task<string> GetShopDataAsync(string shopInfo);
}
