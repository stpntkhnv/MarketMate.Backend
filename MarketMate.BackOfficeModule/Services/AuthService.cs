using MarketMate.BackOfficeModule.Interfaces;
using Microsoft.AspNetCore.Components;

namespace MarketMate.BackOfficeModule.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;

        public AuthService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
        }

        public string AccessToken { get; set; }
        public string UserName { get; set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

        public async Task<bool> AuthenticateAsync(string login, string password)
        {
            var authModel = new { Login = login, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/authentication/authenticate", authModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResult>();
                AccessToken = result.AccessToken;
                UserName = result.UserName;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            AccessToken = null;
            UserName = null;
            _navigationManager.NavigateTo("/login");
        }
    }

    public class AuthResult
    {
        public string AccessToken { get; set; }
        public string UserName { get; set; }
    }
}


