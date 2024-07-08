using LiteDB;
using MarketMate.Application.Abstractions.HttpClients;
using MarketMate.Application.Abstractions.Services;
using MarketMate.Application.Models;
using MarketMate.Contracts.Models.VkApi;
using MarketMate.Domain.Settings;
using MarketMate.Utilities.Helpers.Interfaces;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace MarketMate.Application.Services;

public class PhotoOrganizer : IPhotoOrganizer
{
    private readonly IVkMethodsApiClient _vkApiClient;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly VkApiSettings _vkApiSettings;

    private readonly Regex _linkRegex = new Regex(@"https:\/\/vk\.com\/wall(?<ownerId>-?\d+)_(?<postId>\d+)", RegexOptions.IgnoreCase);
    private readonly Regex _locationRegex = new Regex(@"(?<location>\b[А-Яа-я]+\b)", RegexOptions.IgnoreCase);
    private readonly Regex _sellerLinkRegex = new Regex(@"https:\/\/vk\.com\/wall-?[0-9_]+", RegexOptions.IgnoreCase);

    public PhotoOrganizer(IVkMethodsApiClient vkApiClient, 
        IUserContextAccessor userContextAccessor,
        VkApiSettings vkApiSettings)
    {
        _vkApiSettings = vkApiSettings;
        _userContextAccessor = userContextAccessor;
        _vkApiClient = vkApiClient;
        _vkApiClient.ConfigureGeneralParams(userContextAccessor.GetVkAccessToken(), _vkApiSettings.ApiVersion);
    }

    public async Task<object> ProcessAsync(string purchaseId, long albumId)
    {
        var photos = await _vkApiClient.GetPhotosAsync(-_userContextAccessor.GetCommunityId(), albumId);
        await ExtractSellersAsync(photos);

        return new { Success = true, Message = "Processing completed" };

        throw new NotImplementedException();
    }

    private async Task ExtractSellersAsync(List<Photo> photos)
    {
        var sellers = new List<Seller>();

        foreach (var photo in photos)
        {
            var match = _linkRegex.Match(photo.Text);
            if (match.Success)
            {
                var ownerId = long.Parse(match.Groups["ownerId"].Value);
                var postId = long.Parse(match.Groups["postId"].Value);

                var seller = sellers.FirstOrDefault(s => s.Id == ownerId);

                if (seller is null)
                {
                    sellers.Add(new Seller
                    {
                        Id = ownerId,
                        Photos = [photo],
                        PostIds = [postId]
                    });
                }
                else
                {
                    seller.Photos.Add(photo);
                    seller.PostIds.Add(postId);
                }
            }
        }

        var postIds = sellers
            .SelectMany(s => s.PostIds.Select(p => $"{s.Id}_{p}"))
            .Distinct()
            .ToList();

        var posts = await _vkApiClient.GetPostsAsync(postIds);

        var sellerIds = sellers
            .Select(s => s.Id)
            .Distinct()
            .ToList();

        var groupIds = sellerIds
            .Where(x => x < 0)
            .Select(x => (Math.Abs(x)).ToString())
            .ToList();

        var userIds = sellerIds
            .Where(x => x > 0);

        var groupInfos = await _vkApiClient.GetGroupsByIdAsync(groupIds);

        foreach (var seller in sellers)
        {
            seller.Posts = seller.PostIds.Select(x => posts.FirstOrDefault(c => c.Id == x)!)
                .ToList();

            if (seller.Id < 0)
            {
                seller.SellerInfo = ConvertToSellerInfo(groupInfos.FirstOrDefault(x => x.Id == Math.Abs(seller.Id))!);
                seller.Name = seller.SellerInfo.Name;
                seller.Description = seller.SellerInfo.Description;
            }
        }

        var sellerGroups = sellers
            .Where(x => x.Id < 0)
            .ToList();

        var textBlocks = sellerGroups
            .SelectMany(x => x.Posts)
            .Select(x => x.Text)
            .Union(sellerGroups.Select(x => x.Name))
            .Union(sellerGroups.Select(x => x.Description))
            .ToList();

        Log.Information("Start names");

        foreach (var seller in sellers)
        {
            Log.Information(seller.Name);
        }

        Log.Information("Start desc");

        foreach (var seller in sellers)
        {
            Log.Information(seller.Description);
        }

        Log.Information("Start posts");

        foreach (var seller in sellers)
        {
            foreach (var post in seller.Posts)
            {
                Log.Information(post.Text);
            }
        }
    }

    private async Task<(string Name, string Description)> GetSellerInfoAsync(long ownerId)
    {
        var accessToken = _userContextAccessor.GetVkAccessToken();
        if (ownerId < 0)
        {
            return (await GetGroupInfoAsync(-ownerId));
        }
        else
        {
            return (await GetUserInfoAsync(ownerId));
        }
    }

    private async Task<(string, string)> GetUserInfoAsync(long userId)
    {
        var url = $"https://api.vk.com/method/users.get?user_ids={userId}&access_token={_userContextAccessor.GetVkAccessToken()}&v=5.131";
        using (var client = new HttpClient())
        {
            var response = await client.GetStringAsync(url);
            var vkResponse = JsonConvert.DeserializeObject<VkResponse<List<UserInfo>>>(response);
            var user = vkResponse.Response[0];
            return ($"{user.FirstName} {user.LastName}", user.LastName);
        }
    }

    private async Task<(string, string)> GetGroupInfoAsync(long groupId)
    {
        var url = $"https://api.vk.com/method/groups.getById?group_id={groupId}&access_token={_userContextAccessor.GetVkAccessToken()}&v=5.199&fields=description";
        using (var client = new HttpClient())
        {
            var response = await client.GetStringAsync(url);
            var vkResponse = JsonConvert.DeserializeObject<VkResponse<List<GroupInfo>>>(response);
            var group = vkResponse.Response[0];
            return (group.Name, group.Description);
        }
    }

    private string ExtractLocation(Seller seller)
    {
        Log.Information(seller.Description);
        Log.Information(seller.Name);

        foreach (var item in seller.Posts)
        {
            Log.Information(item.Text);
        }

        return "";
    }

    private SellerInfo ConvertToSellerInfo(GroupInfo groupInfo)
    {
        return new SellerInfo()
        {
            Description = groupInfo.Description,
            Name = groupInfo.Name
        };
    }

    private SellerInfo ConvertToSellerInfo(UserInfo groupInfo)
    {
        throw new NotImplementedException();
    }

    public class Seller
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<long> PostIds { get; set; }
        public List<Post> Posts { get; set; }
        public List<Photo> Photos { get; set; }
        public SellerInfo SellerInfo { get; set; }
    }
}