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

    public override async Task<Response> Tickers(Empty request, ServerCallContext context)
    {
        try
        {
            var response = new Response
            {
                Message = "Ok."
            };

            var queryResult = _tickerRepository.FilterBy(_ => true);
            var data = new Data(){
                Source = "MongoDB"
            };
            foreach (var ticker in queryResult)
            {
                data.TickerList.Add((Ticker) ticker);
            }
            response.Data = data;
            return response;
        }
        catch (Exception e)
        {
            var error = new Error
            {
                Message = e.Message,
                StackTrace = e.StackTrace
            };

            return new Response
            {
                Message = "An error occurred while handling this request.",
                Error = error
            };
        }
    }

    public override async Task<Response> AddTicker(Symbol symbol, ServerCallContext context)
    {
        try
        {
            var response = new Response
            {
                Message = "Successfully created ticker."
            };
            var searchResult = await SearchTickers(symbol.Symbol_);
            var data = new Data();
            if (searchResult.Source != "AlphaVantage")
            {
                data.TickerList.Add(searchResult.Ticker);
                data.Source = searchResult.Source;
                response.Data = data;
                return response;
            }

            var ticker = searchResult.Ticker;
            await _tickerRepository.InsertUniqueAsync(_ => _.Symbol == ticker.Symbol, (MongoDB.Ticker)ticker);
            await _timeSeriesService.AddTimeSeries(ticker.Symbol);

            data.TickerList.Add(ticker);
            data.Source = searchResult.Source;
            response.Data = data;

            return response;

        }
        catch (Exception e)
        {

            var error = new Error
            {
                Message = e.Message,
                StackTrace = e.StackTrace
            };

            return new Response
            {
                Message = "An error occurred while handling this request.",
                Error = error
            };
        }
    }

    public async Task<(string Source, Ticker? Ticker)> SearchTickers(string symbol)
    {
        var mongoResult = await SearchMongo(symbol);
        if (mongoResult != null)
        {
            return (Source: "MongoDB", Ticker: mongoResult);
        }

        var alphaVantageResult = await SearchAlphaVantage(symbol);
        if (alphaVantageResult != null)
        {
            return (Source: "AlphaVantage", Ticker: alphaVantageResult);
        }
        return (Source: "Not found", Ticker: null);
    }

    private async Task<Ticker?> SearchMongo(string symbol)
    {
        var mongoQueryResult = _tickerRepository.FindOneAsync(_ => _.Symbol == symbol);
        if (await mongoQueryResult != null)
        {
            return (Ticker)await mongoQueryResult;
        }
        return null;
    }

    private async Task<Ticker?> SearchAlphaVantage(string symbol)
    {
        var overviewResult = await _requestHandler.Handle<AlphaVantage.Ticker>(ApiFunction.OVERVIEW, new Dictionary<string, string>() { { "symbol", symbol } });
        if (overviewResult != null)
        {
            return (Ticker)overviewResult;
        }
        var symbolSearchResult = await _requestHandler.Handle<SearchTicker>(ApiFunction.SYMBOL_SEARCH, new Dictionary<string, string>() { { "keywords", symbol } });
        if (symbolSearchResult.BestMatches.Any())
        {
            return (Ticker)symbolSearchResult.BestMatches.First();
        }
        return null;
    }
}