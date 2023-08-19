using MongoDB.Bson;

namespace LClaproth.MyFinancialTracker.Portfolio.MongoDB;

[BsonCollection("tickers")]
public class Ticker : AlphaVantage.Ticker, IDocument
{
    public ObjectId Id { get; set; }
    public DateTime CreatedAt => throw new NotImplementedException();
    public string ISIN { get; set; }

    public static explicit operator Portfolio.Ticker.Ticker(Ticker ticker){
        return new Portfolio.Ticker.Ticker {
            Id = ticker.Id.ToString(),
            ISIN = ticker.ISIN,
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
