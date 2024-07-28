using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using MarketMate.Application.Abstractions.Services;
using Polly;
using Polly.Retry;
using Serilog;

namespace MarketMate.Application.Services
{
    public class SellersDataGenerator : ISellersDataGenerator
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly AsyncRetryPolicy<HttpResponseMessage> retryPolicy;
        private static readonly AsyncRetryPolicy<HttpResponseMessage> delayPolicy;

        static SellersDataGenerator()
        {
            retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (result, timeSpan, retryCount, context) =>
                    {
                        Log.Warning("Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}",
                            result.Result.StatusCode, timeSpan, retryCount);
                    });

            delayPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.IsSuccessStatusCode)
                .WaitAndRetryAsync(1, _ => TimeSpan.FromSeconds(new Random().Next(1, 1)),
                    (_, __, ___, ____) => { /* Just to add delay between successful requests */ });
        }

        public async Task<List<SupplierInfo>> GetSupplierInfoFromSadovodBaseAsync(int pageFrom, int pageTo)
        {
            var suppliers = new List<SupplierInfo>();

            try
            {
                for (int page = pageFrom; page <= pageTo; page++)
                {
                    var pageUrl = page == 1 ? "https://sadovod-base.ru/sellers/" : $"https://sadovod-base.ru/sellers/page/{page}";
                    var suppliersFromPage = await FetchSupplierInfoFromPageAsync(pageUrl);
                    suppliers.AddRange(suppliersFromPage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching supplier list");
            }

            return suppliers;
        }

        private async Task<List<SupplierInfo>> FetchSupplierInfoFromPageAsync(string url)
        {
            var suppliers = new List<SupplierInfo>();

            try
            {
                Log.Information("Fetching supplier list from {Url}", url);

                var response = await retryPolicy.ExecuteAsync(() => client.GetAsync(url));
                response.EnsureSuccessStatusCode();

                var pageContents = await response.Content.ReadAsStringAsync();
                Log.Information("Fetched {Length} characters from {Url}", pageContents.Length, url);

                var doc = new HtmlDocument();
                doc.LoadHtml(pageContents);

                var sellerNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'seller-item')]");

                if (sellerNodes != null)
                {
                    foreach (var sellerNode in sellerNodes)
                    {
                        var locationNode = sellerNode.SelectSingleNode(".//ul[contains(@class, 'seller-item__details')]/li");
                        var linkNode = sellerNode.SelectSingleNode(".//a[contains(@class, 'seller-item__title')]");
                        var vkLinkNode = sellerNode.SelectSingleNode(".//a[contains(@class, 'btn btn-outline-primary d-block')]");

                        if (locationNode != null && linkNode != null && vkLinkNode != null)
                        {
                            var vkUrl = vkLinkNode.GetAttributeValue("href", string.Empty).Replace("/redirect/?url=", "").Trim();
                            var vkId = ExtractVkId(vkUrl);

                            var supplierInfo = new SupplierInfo
                            {
                                Id = vkId,
                                VkLink = vkUrl,
                                Location = DetectLocation(locationNode.InnerText.Trim()).Place
                            };

                            Log.Information("Found supplier: {Id}, {VkLink}, {Location}", supplierInfo.Id, supplierInfo.VkLink, supplierInfo.Location);

                            suppliers.Add(supplierInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching supplier list from {Url}", url);
            }

            return suppliers;
        }

        private (string? Building, string? Line, string? Place) DetectLocation(string location)
        {
            location = location.Replace("ТК Садовод, ", string.Empty);
            var isInBuilding = location.Contains("Корпус");
            if (isInBuilding)
            {

            }

            return ("","","");
        }

        private string ExtractVkId(string vkUrl)
        {
            var uri = new Uri(vkUrl);
            var path = uri.AbsolutePath;
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
            {
                throw new InvalidOperationException("Invalid VK URL");
            }

            var id = segments.Last();
            if (id.StartsWith("public"))
            {
                id = "-" + id.Substring("public".Length);
            }
            else if (id.StartsWith("club"))
            {
                id = "-" + id.Substring("club".Length);
            }
            else if (id.StartsWith("id"))
            {
                id = id.Substring("id".Length);
            }

            return id;
        }

        public async Task SaveSuppliersToCsvAsync(List<SupplierInfo> suppliers, string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    await csv.WriteRecordsAsync(suppliers);
                }

                Log.Information("Suppliers data saved to {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while saving suppliers data to CSV");
            }
        }
    }

    public class SupplierInfo
    {
        public string Id { get; set; }
        public string VkLink { get; set; }
        public string Location { get; set; }
    }
}
