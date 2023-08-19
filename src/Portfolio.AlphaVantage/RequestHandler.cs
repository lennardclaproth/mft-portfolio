using Microsoft.Extensions.Options;

namespace LClaproth.MyFinancialTracker.Portfolio.AlphaVantage;

public enum ApiFunction {
    SYMBOL_SEARCH,
    OVERVIEW,
    TIME_SERIES_DAILY
}

public class RequestHandler {

    private readonly HttpClient _client = new HttpClient();
    private readonly AlphaVantageConfiguration _config;
    public RequestHandler(IOptions<AlphaVantageConfiguration> config){
        _config = config.Value;
        _client.BaseAddress = new Uri(_config.BaseUrl);
    }

    public async Task<T>? Handle<T>(ApiFunction apiFunction, Dictionary<string, string> requestParameters) {
        string urlQuery = QueryBuilder.Build(apiFunction, requestParameters, _config.ApiKey);
        var response = await _client.GetAsync(urlQuery);
        var content = await response.Content.ReadAsStringAsync();
        if(content == "{}"){
            return default;
        }
        return await response.Content.ReadFromJsonAsync<T?>();
    }
}