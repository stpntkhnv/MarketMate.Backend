using MarketMate.Application.Services;

namespace MarketMate.Application.Abstractions.Services;

public interface ISellersDataGenerator
{
    Task<List<SupplierInfo>> GetSupplierInfoFromSadovodBaseAsync(int pageFrom, int pageTo);
    Task SaveSuppliersToCsvAsync(List<SupplierInfo> suppliers, string filePath);
}
