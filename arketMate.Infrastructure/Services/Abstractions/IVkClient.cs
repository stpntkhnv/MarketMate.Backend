using MarketMate.Domain.Models;
using MarketMate.Infrastructure.Models;

namespace MarketMate.Infrastructure.Services.Abstractions;

public interface IVkClient
{
    public Task<VkApiResponse<ProfileInfo>> GetProfileInfoAsync();
}
