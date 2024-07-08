using Newtonsoft.Json;

namespace MarketMate.Domain.Models;

public class ProfileInfo
{
    [JsonProperty("first_name")]
    public string FirstName { get; set; }
    [JsonProperty("last_name")]
    public string LastName { get; set; }
    [JsonProperty("maiden_name")]
    public string MaidenName { get; set; }
    [JsonProperty("screen_name")]
    public string ScreenName { get; set; }
}
