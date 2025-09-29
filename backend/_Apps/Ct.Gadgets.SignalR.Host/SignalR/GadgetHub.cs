using Microsoft.AspNetCore.SignalR;

namespace Ct.Gadgets.SignalR.Host.SignalR
{
    public class GadgetHub : Hub
    {
        public async Task SubscribeToGadgets(string[] gadgetIds)
        {
            foreach (var id in gadgetIds)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"gadget-{id}");
        }

        public async Task UnsubscribeFromGadgets(string[] gadgetIds)
        {
            foreach (var id in gadgetIds)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"gadget-{id}");
        }
    }
}
