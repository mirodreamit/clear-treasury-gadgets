using CT.Application.Interfaces;

namespace CT.Tests.Services;

public class GadgetNotifier: IGadgetNotifier
{
    public GadgetNotifier()
    {
    }

    public async Task<bool> NotifyStockChangeAsync(string gadgetId, int newStockQuantity)
    {
        return await Task.FromResult(true);
    }
}
