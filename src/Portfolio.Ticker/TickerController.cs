using Microsoft.AspNetCore.Mvc;

namespace LClaproth.MyFinancialTracker.Portfolio.Ticker;

[ApiController, Route("[controller]")]
public class TickerController : ControllerBase
{
    private readonly TickerService _tickerService;
    public TickerController(TickerService tickerService)
    {
        _tickerService = tickerService;
    }

    // [HttpPost, Route("Add")]
    // public async Task<IActionResult> AddTicker(string symbol){
    //     var ticker = await _tickerService.AddTicker(symbol);
    //     return StatusCode(201, ticker);
    // }
}
