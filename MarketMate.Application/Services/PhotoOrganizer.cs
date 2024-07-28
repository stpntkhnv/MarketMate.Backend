using Google.Cloud.Location;
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
using System.Reactive.Joins;
using System.Text;
using System.Text.RegularExpressions;

namespace MarketMate.Application.Services;

public class PhotoOrganizer : IPhotoOrganizer
{
    private readonly IVkMethodsApiClient _vkApiClient;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly VkApiSettings _vkApiSettings;

    private readonly List<string> _patterns = new List<string>
        {
            // 3) Линия с обозначением

        @"(?<prefix>лин-|линия|л-|л|лин)?(?<line>\d+)(?<suffix>-лин|-линия|-л)?-(?<place>\d+(-\d+)*)",
            // 2) Несколько чисел через дефис
            // 4) Линия с СТ (без дефиса и через дефис)
            @"(?<line>СТ\d+)-(?<place>\d+)",
            @"(?<line>СТ)-(?<lineNum>\d+)-(?<place>\d+)",

        @"(?<line>\d+)-(?<places>\d+(-\d+)+)",
            // 1) Просто два числа через дефис
            @"(?<line>\d+)-(?<place>\d+)",
            
            
        };

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
            .Where(x => x > 0)
            .ToList();

        var groupInfos = await _vkApiClient.GetGroupsByIdAsync(groupIds);

        var userInfos = await _vkApiClient.GetUsersListAsync(userIds);

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
            else
            {
                seller.SellerInfo = ConvertToSellerInfo(userInfos.FirstOrDefault(x => x.Id == seller.Id));
                seller.Name = seller.SellerInfo.Name;
                seller.Description = seller.SellerInfo.Description;
            }
        }

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "output.txt");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var seller in sellers)
            {
                var locName = ExtractLocation(seller.Name);
                var locDesc = ExtractLocation(seller.Description);
                var locsPosts = new List<(string Line, string[] Places)>();
                foreach (var post in seller.Posts)
                {
                    locsPosts.Add(ExtractLocation(post.Text));
                }

                writer.WriteLine($"Продавец: {seller.Name} -> Линия: {locName.Line}, Павильоны: {string.Join(", ", locName.Places)}");

                writer.WriteLine($"Описание: {seller.Description} -> Линия: {locDesc.Line}, Павильоны: {string.Join(", ", locDesc.Places)}");
                foreach (var post in seller.Posts)
                {
                    var locPost = ExtractLocation(post.Text);
                    writer.WriteLine($"Пост: {post.Text} -> Линия: {locPost.Line}, Павильоны: {string.Join(", ", locPost.Places)}");
                }
                writer.WriteLine();
            }
        }
    }

    private (string? Line, string[] Places) ExtractLocation(string locationStrInput)
    {
        if (string.IsNullOrWhiteSpace(locationStrInput))
        {
            return (null, null);
        }
        var locationStr = RemoveSpecificNumbers(locationStrInput);
        locationStr = PrepareString(locationStr);
        locationStr = ReplaceDelimiters(locationStr);

        string line = null;
        List<string> pavilions = new List<string>();

        foreach (var pattern in _patterns)
        {
            var match = Regex.Match(locationStr, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                line = match.Groups["line"].Value;
                var placesGroup = match.Groups["places"];
                if (placesGroup.Success)
                {
                    var places = placesGroup.Value.Split('-');
                    pavilions.AddRange(places);
                }
                else
                {
                    var place = match.Groups["place"].Value;
                    pavilions.AddRange(place.Split('-'));
                }
                break;
            }
        }

        if (line is not null && pavilions.Any())
        {
            return (line, pavilions.ToArray());
        }

        return (line, pavilions.ToArray());
    }

    static string ReplaceDelimiters(string input)
    {
        return Regex.Replace(input, @"[\s.:/]+", "-");
    }

    static string RemoveSpecificNumbers(string input)
    {
        return input.Replace("1000", "").Replace("1001", "");
    }

    private SellerInfo ConvertToSellerInfo(GroupInfo groupInfo)
    {
        return new SellerInfo()
        {
            Description = groupInfo.Description,
            Name = groupInfo.Name
        };
    }

    private SellerInfo ConvertToSellerInfo(UserInfo userInfo)
    {
        return new SellerInfo
        {
            Description = userInfo.About,
            Name = $"{userInfo.FirstName} {userInfo.LastName}",
        };
    }

    static string PrepareString(string input)
    {
        StringBuilder sb = new StringBuilder();
        bool lastWasDigit = false;

        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                if (!lastWasDigit)
                {
                    continue; // Remove space if it's not between digits
                }
            }
            else
            {
                if (char.IsDigit(c))
                {
                    lastWasDigit = true;
                }
                else
                {
                    lastWasDigit = false;
                }
                sb.Append(c);
            }
        }

        return sb.ToString();
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