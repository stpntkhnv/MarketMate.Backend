using MarketMate.Infrastructure.Services.Abstractions;
using Newtonsoft.Json;

namespace MarketMate.Infrastructure.Services;

public class HttpClientServiceBase : IHttpClientService
{
    private readonly HttpClient _httpClient;

    public HttpClientServiceBase()
    {
        _httpClient = new HttpClient();
    }

    public Task<(T Result, string RawResponse)> GetAsync<T>(string url, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null)
    {
        return SendAsync<T>(HttpMethod.Get, url, headers: headers, queryParameters: queryParameters);
    }

    public Task<(T Result, string RawResponse)> PostAsync<T>(string url, object data, Dictionary<string, string>? headers = null)
    {
        return SendAsync<T>(HttpMethod.Post, url, data, headers);
    }

    public Task<(T Result, string RawResponse)> PutAsync<T>(string url, object data, Dictionary<string, string>? headers = null)
    {
        return SendAsync<T>(HttpMethod.Put, url, data, headers);
    }

    public Task<(T Result, string RawResponse)> DeleteAsync<T>(string url, Dictionary<string, string>? headers = null)
    {
        return SendAsync<T>(HttpMethod.Delete, url, headers: headers);
    }

    private async Task<(T Result, string RawResponse)> SendAsync<T>(
        HttpMethod method,
        string url,
        object? data = null,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? queryParameters = null)
    {
        if (queryParameters != null && queryParameters.Count > 0)
        {
            url += ToQueryString(queryParameters);
        }

        var request = new HttpRequestMessage(method, url);

        AddHeaders(request, headers);

        if (data != null)
        {
            request.Content = new ObjectContent<object>(data, new System.Net.Http.Formatting.JsonMediaTypeFormatter());
        }

        HttpResponseMessage response = await _httpClient.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        T result = JsonConvert.DeserializeObject<T>(responseBody);

        return (result, responseBody);
    }

    public async void SetGlobalHeaders(Dictionary<string, string> headers)
    {
        foreach (var header in headers)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    private void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
    {
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
    }

    private string ToQueryString(Dictionary<string, string> queryParameters)
    {
        var queryString = string.Join("&", queryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        return "?" + queryString;
    }
}
