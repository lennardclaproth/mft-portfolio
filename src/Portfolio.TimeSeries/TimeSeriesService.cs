using LClaproth.MyFinancialTracker.Portfolio.MongoDB;
using LClaproth.MyFinancialTracker.Portfolio.AlphaVantage;

namespace LClaproth.MyFinancialTracker.Portfolio.TimeSeries;

public class TimeSeriesService
{

    private readonly RequestHandler _requestHandler;
    private readonly IMongoRepository<MongoDB.TimeSeries> _repository;
    private readonly Func<string, Task<AlphaVantage.TimeSeries?>> _request;

    public TimeSeriesService(
        RequestHandler requestHandler,
        IMongoRepository<MongoDB.TimeSeries> repository)
    {
        _requestHandler = requestHandler;
        _repository = repository;
        _request = symbol => _requestHandler.Handle<AlphaVantage.TimeSeries>(
            ApiFunction.TIME_SERIES_DAILY,
            new Dictionary<string, string>() {
                { "symbol", symbol },
                { "outputsize", "full" }
            });
    }

    public async Task<bool> AddTimeSeries(string symbol)
    {
        var timeSeries = await GetTimeSeries(symbol);
        if(timeSeries != null){
            await UpdateTimeSeries(timeSeries);
            return true;
        }
        var requestResult = (MongoDB.TimeSeries) await _request(symbol);
        if (requestResult == null)
        {
            return false;
        }
        _repository.InsertUniqueAsync(_ => _.Symbol == requestResult.Symbol, requestResult);
        return true;
    }

    public async Task<MongoDB.TimeSeries?> GetTimeSeries(string symbol)
    {
        return await _repository.FindOneAsync(_ => _.Symbol == symbol);
    }

    private async Task<bool> UpdateTimeSeries(MongoDB.TimeSeries timeSeries)
    {
        if(timeSeries.LastRefreshed.CompareTo(DateTime.Today.AddDays(-1)) >= 0){
            return false;
        }
        
        var requestResult = await _request(timeSeries.Symbol);

        if(requestResult == null){
            return false;
        }
        timeSeries = new MongoDB.TimeSeries {
            Symbol = timeSeries.Symbol,
            Dailies = requestResult.Dailies,
            LastRefreshed = requestResult.MetaData.LastRefreshed,
        };
        await _repository.ReplaceOneAsync(timeSeries);
        return true;
    }
}
