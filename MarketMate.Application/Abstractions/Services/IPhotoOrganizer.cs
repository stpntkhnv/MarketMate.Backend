namespace MarketMate.Application.Abstractions.Services;

public interface IPhotoOrganizer
{
    Task<object> ProcessAsync(string purchaseId, long albumId);
}
