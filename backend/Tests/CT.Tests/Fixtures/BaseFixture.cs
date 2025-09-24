using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CT.Tests.Extensions;

namespace CT.Tests.Fixtures;

public class BaseFixture
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly IMediator _mediator;

    public BaseFixture()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTestServices();

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _mediator = _serviceProvider.GetRequiredService<IMediator>();
    }
}
