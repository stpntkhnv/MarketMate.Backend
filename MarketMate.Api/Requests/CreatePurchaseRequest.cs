namespace MarketMate.Api.Requests;

public class CreatePurchaseRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public long CommunityId { get; set; }
    public long AlbumId { get; set; }
    public long AgentId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
