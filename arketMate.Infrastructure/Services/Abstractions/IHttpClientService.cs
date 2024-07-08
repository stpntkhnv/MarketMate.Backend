namespace MarketMate.Infrastructure.Services.Abstractions;

public interface IHttpClientService
{
    Task<(T Result, string RawResponse)> GetAsync<T>(string url, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null);
    Task<(T Result, string RawResponse)> PostAsync<T>(string url, object data, Dictionary<string, string>? headers = null);
    Task<(T Result, string RawResponse)> PutAsync<T>(string url, object data, Dictionary<string, string>? headers = null);
    Task<(T Result, string RawResponse)> DeleteAsync<T>(string url, Dictionary<string, string>? headers = null);
    void SetGlobalHeaders(Dictionary<string, string> headers);
}
