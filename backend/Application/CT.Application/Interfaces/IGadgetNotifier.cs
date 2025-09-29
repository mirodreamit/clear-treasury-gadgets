namespace CT.Application.Interfaces;

public interface IGadgetNotifier
{
    Task<bool> NotifyStockChangeAsync(string gadgetId, int stockQuantity);
}
