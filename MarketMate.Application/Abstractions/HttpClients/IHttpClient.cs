namespace MarketMate.Application.Abstractions.HttpClients;

public interface IHttpClient
{
    Task<T> GetAsync<T>(string endpoint);
    Task<T> PostAsync<T>(string endpoint, HttpContent content);
    Task<T> PutAsync<T>(string endpoint, HttpContent content);
    Task DeleteAsync(string endpoint);
}
