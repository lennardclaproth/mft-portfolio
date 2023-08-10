using System.Text.Json.Serialization;

namespace MyFinancialTracker.Portfolio.AlphaVantage;

public class SearchTicker
{
    public IEnumerable<BestMatches> BestMatches { get; set; }
}

public class BestMatches 
{
    [JsonPropertyName("1. symbol")]
    public string Symbol {get; set; }

    [JsonPropertyName("2. name")]
    public string Name {get; set; }

    [JsonPropertyName("3. type")]
    public string? AssetType {get; set; }

    [JsonPropertyName("8. currency")]
    public string? Currency {get; set; }

     public static explicit operator Portfolio.Ticker.Ticker(BestMatches ticker){
        return new Portfolio.Ticker.Ticker
        {
            Symbol = ticker.Symbol,
            Name = ticker.Name,
            AssetType = ticker.AssetType,
            Currency = ticker.Currency
        };
    }
}
