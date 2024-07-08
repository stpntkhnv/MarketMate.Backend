using VkNet.Model;

namespace MarketMate.Application.Abstractions;

public interface IVkService
{
    Task<IEnumerable<PhotoAlbum>> GetGroupPhotoAlbumsAsync(long groupId);
    Task<IEnumerable<Photo>> GetAlbumPhotosAsync(long albumId, long groupId);
    Task<long> CreatePhotoAlbumAsync(long groupId, string title);
    Task MovePhotoToAlbumAsync(long photoId, ulong albumId, long groupId);
    Task<IEnumerable<Group>> GetUserGroupsAsync();
}
