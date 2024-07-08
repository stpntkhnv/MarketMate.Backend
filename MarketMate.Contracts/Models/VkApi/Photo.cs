namespace MarketMate.Contracts.Models.VkApi;

public class Photo
{
    public int Id { get; set; }
    public int AlbumId { get; set; }
    public int OwnerId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; }
    public int Date { get; set; }
    public List<PhotoSize> Sizes { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? CanComment { get; set; }
    public Comments Comments { get; set; }
    public Likes Likes { get; set; }
    public Reposts Reposts { get; set; }
}
