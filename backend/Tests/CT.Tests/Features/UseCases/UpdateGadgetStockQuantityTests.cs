using CT.Application.Features.Categories.Commands;
using CT.Application.Features.Gadgets.Commands;
using CT.Application.Features.Gadgets.Queries;
using CT.Tests.Fixtures;

namespace CT.Tests.Features.UseCases;
public class UpdateGadgetStockQuantityTests : BaseFixture
{
    private const int InitialStockQuantity = 10;

    [Fact]
    public async void ShouldCreateAGadgetAndIncreseDecreaseItsStockQuantity()
    {
        var gadgetId = await CreateAGadgetAsync();

        var cmdIncrease = new IncreaseGadgetStockQuantityCommand(gadgetId);

        int i = 1;
        var tasksIncrease = Enumerable.Range(0, i)
                    .Select(_ => _mediator.Send(cmdIncrease))
                    .ToArray();

        await Task.WhenAll(tasksIncrease);

        var cmdDescrease = new DecreaseGadgetStockQuantityCommand(gadgetId);

        var tasksDecrease = Enumerable.Range(0, i)
                    .Select(_ => _mediator.Send(cmdDescrease))
                    .ToArray();

        await Task.WhenAll(tasksDecrease);

        var q = new GetGadgetByIdQuery(gadgetId);
        var qRes = await _mediator.Send(q);
        var stockQuantity = qRes.Model!.StockQuantity;

        // cleanup
        await DeleteGadgetsAsync([gadgetId]);

        Assert.True(stockQuantity == InitialStockQuantity);
    }

    private async Task<Guid> CreateAGadgetAsync()
    {
        var cmd = new UpsertGadgetCommand(Guid.NewGuid(), new UpsertGadgetCommand.CreateGadgetRequestModel()
        {
            Name = "This is a test Gadget 111",
            StockQuantity = InitialStockQuantity
        });

        var res = await _mediator.Send(cmd);

        return res.Model!.Id;
    }

    private async Task DeleteGadgetsAsync(List<Guid> ids)
    {
        foreach (var id in ids)
        {
            var del = new DeleteGadgetCommand(id);

            await _mediator.Send(del);
        }
    }

}
