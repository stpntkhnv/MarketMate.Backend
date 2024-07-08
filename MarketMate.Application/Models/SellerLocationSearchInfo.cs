namespace MarketMate.Application.Models;

public class SellerLocationSearchInfo
{
    public long Id { get; set; } 
    public string Name { get; set; } 
    public string Description { get; set; } 
    public string PostDescription { get; set; } 
    public string ShopLink { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is SellerLocationSearchInfo other)
        {
            return Id == other.Id &&
                   Name == other.Name &&
                   Description == other.Description &&
                   PostDescription == other.PostDescription &&
                   ShopLink == other.ShopLink;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Description, PostDescription, ShopLink);
    }
}