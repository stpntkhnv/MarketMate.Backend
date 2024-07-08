using Newtonsoft.Json;

namespace MarketMate.Infrastructure.Models;

public class VkApiResponse<T>
{
    [JsonProperty("response")]
    public T Response { get; set; }

    [JsonIgnore]
    public string RawResponse { get; set; }
}
