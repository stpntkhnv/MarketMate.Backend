using Mapster;
using MarketMate.Application.Abstractions.Services;
using MarketMate.Application.Dtos;
using MarketMate.Domain.Abstractions.Repositories;
using MarketMate.Domain.Entities;

namespace MarketMate.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IRepository<Purchase> _purchasesRepository;

    public PurchaseService(IRepository<Purchase> purchasesRepository)
    {
        _purchasesRepository = purchasesRepository;        
    }

    public async Task<PurchaseDto> CreateAsync(PurchaseDto dto)
    {
        var entity = await _purchasesRepository.AddAsync(dto.Adapt<Purchase>());
        return entity.Adapt<PurchaseDto>();
    }

    public async Task DeleteAsync(string id)
    {
        await _purchasesRepository.DeleteAsync(id);
    }

    public async Task<ICollection<PurchaseDto>> GetAllAsync()
    {
        return (await _purchasesRepository.GetAllAsync()).Adapt<List<PurchaseDto>>();
    }

    public async Task<PurchaseDto> GetByIdAsync(string id)
    {
        return (await _purchasesRepository.GetByIdAsync(id)).Adapt<PurchaseDto>();
    }
}
