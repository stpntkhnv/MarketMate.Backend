namespace MarketMate.Contracts.Models.VkApi;

public class GroupInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ScreenName { get; set; }
    public int IsClosed { get; set; }
    public string Type { get; set; }
    public int IsAdmin { get; set; }
    public int IsMember { get; set; }
    public int IsAdvertiser { get; set; }
    public string Photo50 { get; set; }
    public string Photo100 { get; set; }
    public string Photo200 { get; set; }
}
