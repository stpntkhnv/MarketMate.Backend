using MarketMate.Domain.Models;
using MarketMate.Domain.Settings;
using MarketMate.Infrastructure.Models;
using MarketMate.Infrastructure.Services.Abstractions;
using MarketMate.Utilities.Helpers.Interfaces;

namespace MarketMate.Infrastructure.Services;

public class VkClient : IVkClient
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly VkApiSettings _vkApiSettings;

    private readonly string _accessToken;

    public VkClient(
        IHttpClientService httpClientService, 
        IUserContextAccessor userContextAccessor, 
        VkApiSettings vkApiSettings)
    {
        _userContextAccessor = userContextAccessor;
        _httpClientService = httpClientService;
        _accessToken = _userContextAccessor.GetVkAccessToken();
        _httpClientService.SetGlobalHeaders(new Dictionary<string, string>
        {
            {"Authorization", $"Bearer {_accessToken}"}
        });
        _vkApiSettings = vkApiSettings;
    }

    public async Task<VkApiResponse<ProfileInfo>> GetProfileInfoAsync()
    {
        var (result, rawResponse)= await _httpClientService.GetAsync<VkApiResponse<ProfileInfo>>("https://api.vk.com/method/account.getProfileInfo", null, new Dictionary<string, string>
        {
            {"v", _vkApiSettings.ApiVersion}
        });

        return new VkApiResponse<ProfileInfo>()
        {
            RawResponse = rawResponse,
            Response = result.Response
        };
    }
}
