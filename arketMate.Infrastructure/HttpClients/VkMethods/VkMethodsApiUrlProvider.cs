using MarketMate.Infrastructure.Extensions;

namespace MarketMate.Infrastructure.HttpClients.VkMethods;

public static class VkMethodsApiUrlProvider
{
    public static string GetPhotosUrl(
        string ownerId,
        string albumId,
        string accessToken,
        string apiVersion,
        string count = "50",
        string extended = "0",
        string rev = "1")
    {
        return VkMethodsApiEndpointUrls.PhotosGet
            .AddQueryParameter(VkMethodsApiParameterNames.OwnerId, ownerId)
            .AddQueryParameter(VkMethodsApiParameterNames.AlbumId, albumId)
            .AddQueryParameter(VkMethodsApiParameterNames.Extended, extended)
            .AddQueryParameter(VkMethodsApiParameterNames.Rev, rev)
            .AddQueryParameter(VkMethodsApiParameterNames.AccessToken, accessToken)
            .AddQueryParameter(VkMethodsApiParameterNames.ApiVersion, apiVersion);
    }

    public static string GetPostsUrl(
        string posts,
        string accessToken,
        string apiVersion,
        string extended = "0",
        string copyHistoryDepth = null,
        string fields = null)
    {
        var url = VkMethodsApiEndpointUrls.WallGetById
            .AddQueryParameter(VkMethodsApiParameterNames.Posts, posts)
            .AddQueryParameter(VkMethodsApiParameterNames.AccessToken, accessToken)
            .AddQueryParameter(VkMethodsApiParameterNames.ApiVersion, apiVersion)
            .AddQueryParameter(VkMethodsApiParameterNames.Extended, extended);

        if (!string.IsNullOrEmpty(copyHistoryDepth))
        {
            url = url.AddQueryParameter(VkMethodsApiParameterNames.CopyHistoryDepth, copyHistoryDepth);
        }

        if (!string.IsNullOrEmpty(fields))
        {
            url = url.AddQueryParameter(VkMethodsApiParameterNames.Fields, fields);
        }

        return url;
    }

    public static string GetGroupsUrl(
        string groupIds,
        string accessToken,
        string apiVersion,
        string fields = null)
    {
        var url = VkMethodsApiEndpointUrls.GroupsGetById
            .AddQueryParameter(VkMethodsApiParameterNames.GroupIds, groupIds)
            .AddQueryParameter(VkMethodsApiParameterNames.AccessToken, accessToken)
            .AddQueryParameter(VkMethodsApiParameterNames.ApiVersion, apiVersion);

        if (!string.IsNullOrEmpty(fields))
        {
            url = url.AddQueryParameter(VkMethodsApiParameterNames.Fields, fields);
        }

        return url;
    }

    public static string GetUploadPhotosServerUrl(
        string albumId,
        string accessToken,
        string apiVersion,
        string? groupId = null)
    {
        var url = VkMethodsApiEndpointUrls.PhotosGetUploadServer
            .AddQueryParameter(VkMethodsApiParameterNames.AlbumId, albumId)
            .AddQueryParameter(VkMethodsApiParameterNames.AccessToken, accessToken)
            .AddQueryParameter(VkMethodsApiParameterNames.ApiVersion, apiVersion);

        if (!string.IsNullOrEmpty(groupId)) 
        {
            url = url.AddQueryParameter(VkMethodsApiParameterNames.GroupId, groupId);
        }

        return url;
    }

    public static string GetEditPhotoUrl(
        string ownerId,
        string photoId,
        string description,
        string accessToken,
        string apiVersion)
    {
        var url = VkMethodsApiEndpointUrls.PhotosEdit
            .AddQueryParameter(VkMethodsApiParameterNames.OwnerId, ownerId)
            .AddQueryParameter(VkMethodsApiParameterNames.PhotoId, photoId)
            .AddQueryParameter(VkMethodsApiParameterNames.Caption, description)
            .AddQueryParameter(VkMethodsApiParameterNames.AccessToken, accessToken)
            .AddQueryParameter(VkMethodsApiParameterNames.ApiVersion, apiVersion);

        return url;
    }

    public static string GetUsersListUrl(
        string userIds,
        string accessToken,
        string apiVersion,
        string fields = "description",
        string nameCase = "nom")
    {
        var url = VkMethodsApiEndpointUrls.UsersGet
            .AddQueryParameter(VkMethodsApiParameterNames.UserIds, userIds)
            .AddQueryParameter(VkMethodsApiParameterNames.Fields, fields)
            .AddQueryParameter(VkMethodsApiParameterNames.NameCase, nameCase)
            .AddQueryParameter(VkMethodsApiParameterNames.AccessToken, accessToken)
            .AddQueryParameter(VkMethodsApiParameterNames.ApiVersion, apiVersion);

        return url;
    }
}
