namespace MarketMate.Utilities.Helpers.Interfaces;

public interface IUserContextAccessor
{
    public string GetVkAccessToken();
    public string GetVkUserId();
    public long GetUserId();
    public long GetCommunityId();
}
