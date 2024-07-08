namespace MarketMate.Domain.Abstractions.Services;

public interface IProcurementService
{
    Task<ProcurementDto> CreateProcurementAsync(ProcurementDto request);
    Task<ProcurementDto> GetProcurementAsync(string id);
}
