namespace MarketMate.Application.Abstractions;

public interface IVkAuthService
{
    Task<bool> AuthenticateAsync(string login, string password);
    void SetAccessToken(string accessToken);
    string GetAccessToken();
}
