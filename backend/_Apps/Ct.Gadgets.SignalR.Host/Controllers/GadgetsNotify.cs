using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Ct.Gadgets.SignalR.Host.SignalR;

namespace CT.Gadgets.SignalR.Host.Controllers;

[ApiController]
[Route("api/gadgets")]
public class GadgetController : ControllerBase
{
    private readonly IHubContext<GadgetHub> _hubContext;

    public GadgetController(IHubContext<GadgetHub> hubContext) => _hubContext = hubContext;

    [HttpPost("update-stock")]
    public async Task<IActionResult> UpdateStock([FromBody] GadgetStockQuantity model)
    {
        if (string.IsNullOrWhiteSpace(model.GadgetId))
            return BadRequest("GadgetId is required.");

        // Notify SignalR clients in the relevant group
        //await _hubContext.Clients.Group($"gadget-{model.GadgetId}")
        //    .SendAsync("StockQuantityUpdated", model.GadgetId, model.StockQuantity);

        // Send to all connected clients
        await _hubContext.Clients.All
            .SendAsync("StockQuantityUpdated", model.GadgetId, model.StockQuantity);


        return Ok(new { message = "Stock updated", gadgetId = model.GadgetId, stock = model.StockQuantity });
    }
    public class GadgetStockQuantity
    {
        public string GadgetId { get; set; } = default!;
        public int StockQuantity { get; set; }
    }
}