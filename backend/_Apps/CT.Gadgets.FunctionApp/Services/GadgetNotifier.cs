using CT.Application.Interfaces;

namespace CT.Gadgets.FunctionApp.Services;

internal class GadgetNotifier(GadgetsHubHttpClient gadgetsHubHttpClient) : IGadgetNotifier
{
    private readonly GadgetsHubHttpClient _gadgetsHubHttpClient = gadgetsHubHttpClient;

    public async Task<bool> NotifyStockChangeAsync(string gadgetId, int stockQuantity)
    {
        return await _gadgetsHubHttpClient.NotifyStockChangeAsync(gadgetId, stockQuantity).ConfigureAwait(false);
    }
}
