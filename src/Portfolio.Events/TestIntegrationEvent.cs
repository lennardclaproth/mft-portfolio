using LClaproth.MyFinancialTracker.EventBus;

namespace LClaproth.MyFinancialTracker.Portfolio;

class TestIntegrationEvent : IntegrationEvent
{
    public override string Name => "test-event";
}