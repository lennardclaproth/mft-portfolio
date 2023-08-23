using LClaproth.MyFinancialTracker.EventBus;

namespace LClaproth.MyFinancialTracker.Portfolio;

class TestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
{
    public Task Handle(TestIntegrationEvent @event)
    {
        Console.WriteLine("event fired");
        return Task.CompletedTask;
    }
}