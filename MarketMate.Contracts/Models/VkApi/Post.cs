using Newtonsoft.Json;

namespace MarketMate.Contracts.Models.VkApi;

public class Post
{
    public long Id { get; set; }

    [JsonProperty("owner_id")]
    public long OwnerId { get; set; }
    public int FromId { get; set; }
    public int? CreatedBy { get; set; }
    public int Date { get; set; }
    public string Text { get; set; }
    public int? ReplyOwnerId { get; set; }
    public int? ReplyPostId { get; set; }
    public int? FriendsOnly { get; set; }
    public string PostType { get; set; }
    public int? SignerId { get; set; }
    public int? CanPin { get; set; }
    public int? CanDelete { get; set; }
    public int? CanEdit { get; set; }
    public int? IsPinned { get; set; }
    public int? MarkedAsAds { get; set; }
    public bool? IsFavorite { get; set; }
    public int? PostponedId { get; set; }
}
