namespace MyFinancialTracker.Portfolio.AlphaVantage;

public class Ticker
{
    public string? Symbol {get; set; }
    public string? Name {get; set; }
    public string? Description {get; set; }
    public string? AssetType {get; set; }
    public string? Country {get; set; }
    public string? Sector {get; set; }
    public string? Industry {get; set; }
    public string? Currency {get; set; }
    
    public static explicit operator Portfolio.Ticker.Ticker(Ticker ticker){
        return new Portfolio.Ticker.Ticker
        {
            Symbol = ticker.Symbol,
            Name = ticker.Name,
            Description = ticker.Description,
            AssetType = ticker.AssetType,
            Country = ticker.Country,
            Sector = ticker.Sector,
            Industry = ticker.Industry,
            Currency = ticker.Currency
        };
    }
}
