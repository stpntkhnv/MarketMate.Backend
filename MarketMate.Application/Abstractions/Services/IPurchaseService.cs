using MarketMate.Application.Dtos;

namespace MarketMate.Application.Abstractions.Services;

public interface IPurchaseService
{
    Task<ICollection<PurchaseDto>> GetAllAsync();
    Task<PurchaseDto> GetByIdAsync(string id);
    Task<PurchaseDto> CreateAsync(PurchaseDto dto);
    Task DeleteAsync(string id);
}
