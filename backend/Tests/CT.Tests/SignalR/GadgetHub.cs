using Microsoft.AspNetCore.SignalR;

namespace CT.Tests.SignalR
{
    public class GadgetHub : Hub
    {
        public async Task SubscribeToGadgets(string[] gadgetIds)
        {
            foreach (var id in gadgetIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"gadget-{id}");
            }
        }

        public async Task UnsubscribeFromGadgets(string[] gadgetIds)
        {
            foreach (var id in gadgetIds)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"gadget-{id}");
            }
        }

        // Optional cleanup
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // SignalR automatically removes connection from groups on disconnect,
            // so you usually don’t need manual cleanup here
            await base.OnDisconnectedAsync(exception);
        }
    }

}
