using System.Web;

namespace MarketMate.Infrastructure.Extensions;

public static class UrlExtensions
{
    public static string AddQueryParameters(this string url, Dictionary<string, string> parameters)
    {
        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var param in parameters)
        {
            query[param.Key] = param.Value;
        }

        uriBuilder.Query = query.ToString();

        return uriBuilder.ToString();
    }

    public static string AddQueryParameter(this string url, string parameterName, string parameterValue)
    {
        var uri = new UriBuilder("http://dummy.com" + url); 
        var query = HttpUtility.ParseQueryString(uri.Query);
        query[parameterName] = parameterValue;
        return url.Contains("?")
            ? url + "&" + parameterName + "=" + HttpUtility.UrlEncode(parameterValue)
            : url + "?" + parameterName + "=" + HttpUtility.UrlEncode(parameterValue);
    }
}
