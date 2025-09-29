using CT.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CT.Tests.SignalR;

public class GadgetNotifier: IGadgetNotifier
{
    private readonly IHubContext<GadgetHub> _hubContext;

    public GadgetNotifier(IHubContext<GadgetHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task<bool> NotifyStockChangeAsync(string gadgetId, int newStockQuantity)
    {
        return await Task.FromResult(true);
    }
}
