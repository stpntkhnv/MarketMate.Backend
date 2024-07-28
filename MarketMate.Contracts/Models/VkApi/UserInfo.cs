using Newtonsoft.Json;

namespace MarketMate.Contracts.Models.VkApi;

public class UserInfo
{
    [JsonProperty("id")]
    public long Id { get; set; }
    [JsonProperty("first_name")]
    public string FirstName { get; set; }
    [JsonProperty("last_name")]
    public string LastName { get; set; }

    [JsonProperty("about")]
    public string About { get; set; }
}
