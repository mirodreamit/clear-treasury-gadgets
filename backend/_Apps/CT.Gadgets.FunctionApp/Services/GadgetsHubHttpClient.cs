
using System.Net.Http.Json;

namespace CT.Gadgets.FunctionApp.Services;

public class GadgetsHubHttpClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> NotifyStockChangeAsync(string gadgetId, int stockQuantity)
    {
        var payload = new
        {
            GadgetId = gadgetId,
            StockQuantity = stockQuantity
        };
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/gadgets/update-stock", payload);

            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            throw;
        }

    }
}
