namespace MarketMate.Domain.Abstractions.Services;

public class ProcurementDto
{
    public string Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? StartDate { get; set; }
    public string CreatedUserId { get; set; }
    public string Name { get; set; }
}