namespace MarketMate.BackOfficeModule.Interfaces
{
    public interface IAuthService
    {
        string AccessToken { get; set; }
        string UserName { get; set; }
        bool IsAuthenticated { get; }
        Task<bool> AuthenticateAsync(string login, string password);
        void Logout();
    }
}
