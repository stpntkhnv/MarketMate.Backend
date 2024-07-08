using Google.Protobuf.Reflection;
using MarketMate.Contracts.Models.VkApi;

namespace MarketMate.Application.Abstractions.HttpClients;

public interface IVkMethodsApiClient
{
    Task<List<Photo>> GetPhotosAsync(long ownerId, long albumId, int cout = 50);
    Task<List<Post>> GetPostsAsync(List<string> postIds);
    Task<List<GroupInfo>> GetGroupsByIdAsync(List<string> groupIds);
    Task<UserInfo> GetUserInfoAsync(long userId);
    Task<List<UserInfo>> GetUsersListAsync(List<long> userId);
    Task<GroupInfo> GetGroupInfoAsync(long groupId);
    Task<UploadServer> GetPhotosUploadServerAsync(long albumId, long? groupId);
    Task<string> CreateAlbumAsync(long groupId, string title);
    Task UploadPhotoToAlbumAsync(long albumId, Photo photo);
    Task EditPhotoAsync(long ownerId, long photoId, string description);
    void ConfigureGeneralParams(string accessToken, string apiVersion);
}
