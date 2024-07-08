using MarketMate.Utilities.Helpers.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MarketMate.Utilities.Helpers;

public class UserContextAccessor : IUserContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetVkAccessToken()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["x-mm-vk-access-token"].FirstOrDefault();
        return authorizationHeader ?? string.Empty;
    }

    public string GetVkUserId()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["VkUserId"].FirstOrDefault();
        return authorizationHeader ?? string.Empty;
    }

    public long GetUserId()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["x-mm-user-id"].FirstOrDefault();
        return authorizationHeader is not null ? long.Parse(authorizationHeader) : 0;
    }

    public long GetCommunityId()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["x-mm-community-id"].FirstOrDefault();
        return authorizationHeader is not null ? long.Parse(authorizationHeader) : 0;
    }
}
