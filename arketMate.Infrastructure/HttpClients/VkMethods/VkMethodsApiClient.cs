using MarketMate.Application.Abstractions.HttpClients;
using MarketMate.Contracts.Models.VkApi;
using Serilog;

namespace MarketMate.Infrastructure.HttpClients.VkMethods;

public class VkMethodsApiClient : HttpClientBase, IVkMethodsApiClient
{
    private readonly HttpClient _httpClient;

    private VkApiCommonParameters _commonParameters = new(string.Empty, string.Empty);

    private record VkApiCommonParameters(string AccessToken, string ApiVersion);
    private record VkApiListResponse<T>(List<T> Items);
    private record VkApiGroupsResponse(List<GroupInfo> Groups);

    public VkMethodsApiClient(HttpClient httpClient) : base(httpClient)
    {
        _httpClient = httpClient;
    }

    public void ConfigureGeneralParams(string accessToken, string apiVersion)
    {
        _commonParameters = new(accessToken, apiVersion);
    }

    public async Task<List<Photo>> GetPhotosAsync(long ownerId, long albumId, int count = 50)
    {
        var url = VkMethodsApiUrlProvider.GetPhotosUrl(ownerId.ToString(), albumId.ToString(), _commonParameters.AccessToken, _commonParameters.ApiVersion, count.ToString());

        var response = await GetAsync<VkResponse<PhotoList>>(url);

        return response.Response.Items;
    }

    public async Task<List<Post>> GetPostsAsync(List<string> postIds)
    {
        var url = VkMethodsApiUrlProvider.GetPostsUrl(string.Join(',', postIds), _commonParameters.AccessToken, _commonParameters.ApiVersion);

        var response = await GetAsync<VkResponse<PostList>>(url);

        return response.Response.Items;
    }

    public async Task<List<GroupInfo>> GetGroupsByIdAsync(List<string> groupIds)
    {
        var url = VkMethodsApiUrlProvider.GetGroupsUrl(string.Join(',', groupIds), _commonParameters.AccessToken, _commonParameters.ApiVersion, "description");

        var response = await GetAsync<VkResponse<VkApiGroupsResponse>>(url);

        return response.Response.Groups;
    }

    public async Task<UploadServer> GetPhotosUploadServerAsync(long albumId, long? groupId)
    {
        var url = VkMethodsApiUrlProvider.GetUploadPhotosServerUrl(albumId.ToString(), _commonParameters.AccessToken, _commonParameters.ApiVersion, Math.Abs(groupId.Value).ToString());

        var response = await GetAsync<VkResponse<UploadServer>>(url);

        return response.Response;
    }

    public async Task<GroupInfo> GetGroupInfoAsync(long groupId)
    {
        return (await GetGroupsByIdAsync(new List<string>() { groupId.ToString()})).FirstOrDefault();
    }

    public Task<string> CreateAlbumAsync(long groupId, string title)
    {
        throw new NotImplementedException();
    }

    public async Task<UserInfo> GetUserInfoAsync(long userId)
    {
        var url = VkMethodsApiUrlProvider.GetUsersListUrl(userId.ToString(), _commonParameters.AccessToken, _commonParameters.ApiVersion);

        var response = await GetAsync<VkResponse<List<UserInfo>>>(url);

        return response.Response.FirstOrDefault();
    }

    public Task UploadPhotoToAlbumAsync(long albumId, Photo photo)
    {
        throw new NotImplementedException();
    }

    public async Task EditPhotoAsync(long ownerId, long photoId, string description)
    {
        var url = VkMethodsApiUrlProvider.GetEditPhotoUrl(ownerId.ToString(), photoId.ToString(), description, _commonParameters.AccessToken, _commonParameters.ApiVersion);

        await GetAsync<object>(url);
    }

    public async Task<List<UserInfo>> GetUsersListAsync(List<long> userIds)
    {
        var url = VkMethodsApiUrlProvider.GetUsersListUrl(string.Join(',', userIds), _commonParameters.AccessToken, _commonParameters.ApiVersion);

        Log.Information(url);

        var response = await GetAsync<VkResponse<List<UserInfo>>>(url);
         
        return response.Response;
    }
}
