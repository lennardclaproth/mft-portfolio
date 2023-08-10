using Grpc.Core;
using MongoDB.Driver;
using System.Linq.Expressions;
using MyFinancialTracker.Portfolio.MongoDB;
using MyFinancialTracker.Portfolio.TimeSeries;
using MyFinancialTracker.Portfolio.AlphaVantage;

namespace MyFinancialTracker.Portfolio.Ticker;

public class TickerService : TickerHandler.TickerHandlerBase
{
    private readonly RequestHandler _requestHandler;
    private readonly TimeSeriesService _timeSeriesService;
    private readonly IMongoRepository<MongoDB.Ticker> _tickerRepository;

    public TickerService(
        RequestHandler requestHandler, 
        TimeSeriesService timeSeriesService,
        IMongoRepository<MongoDB.Ticker> tickerRepository)
    {
        _requestHandler = requestHandler;
        _tickerRepository = tickerRepository;
        _timeSeriesService = timeSeriesService;
    }

    public override Task<TickerResponse> Tickers(Empty request, ServerCallContext context)
    {
        var result = new TickerResponse();
        var queryResult = _tickerRepository.FilterBy(_ => true);
        foreach (var ticker in queryResult)
        {
            result.Tickers.Add((Ticker)ticker);
        }
        result.Source = "MongoDB";
        return Task.FromResult(result);
    }

    public async Task<TickerResponse> AddTicker(string symbol)
    {
        var searchResult = await SearchTickers(symbol);

        if (searchResult.Source != "AlphaVantage")
        {
            return searchResult;
        }

        var ticker = searchResult.Tickers.FirstOrDefault();
        var addTicker = _tickerRepository.InsertUniqueAsync(_ => _.Symbol == ticker.Symbol, (MongoDB.Ticker) ticker);
        var addTimeSeries = _timeSeriesService.AddTimeSeries(ticker.Symbol);
        
        await addTicker;
        await addTimeSeries;

        return searchResult; 
    }

    public async Task<TickerResponse>? SearchTickers(string symbol)
    {
        try
        {
            var mongoQueryResult = await SearchMongo(symbol);
            if (mongoQueryResult.Tickers.Any())
            {
                return mongoQueryResult;
            }
            var alphaVantageResult = await SearchAlphaVantage(symbol);
            if (alphaVantageResult.Tickers.Any())
            {
                return alphaVantageResult;
            }
            return new TickerResponse
            {
                Source = "Not Found",
            };
        }
        catch (Exception e)
        {
            return new TickerResponse
            {
                Source = "Error",
                Error = new Error
                {
                    ErrorMessage = e.Message,
                    StackTrace = e.StackTrace
                }
            };
            throw new Exception($"Error occurred while handling request with symbol: {symbol}, nested exception is: ", e);
        }
    }

    private async Task<TickerResponse> SearchMongo(string symbol)
    {
        var mongoQueryResult = _tickerRepository.FindOneAsync(_ => _.Symbol == symbol);
        
        var searchResult = new TickerResponse
        {
            Source = "MongoDB",
        };
        if(await mongoQueryResult != null){
            searchResult.Tickers.Add((Ticker) await mongoQueryResult);
        }
        return searchResult;
    }

    private async Task<TickerResponse> SearchAlphaVantage(string symbol)
    {
        var overviewResult = await _requestHandler.Handle<AlphaVantage.Ticker>(ApiFunction.OVERVIEW, new Dictionary<string, string>() { { "symbol", symbol } });
        var searchResult = new TickerResponse
        {
            Source = "AlphaVantage"
        };
        if(overviewResult != null){
            searchResult.Tickers.Add((Ticker) overviewResult);
            return searchResult;
        }
        var symbolSearchResult = await _requestHandler.Handle<SearchTicker> (ApiFunction.SYMBOL_SEARCH, new Dictionary<string, string>() {{"keywords",symbol}});
        if(symbolSearchResult.BestMatches.Any())
        {
            searchResult.Tickers.Add((Ticker) symbolSearchResult.BestMatches.First());
        }
        return searchResult;
    }
}
