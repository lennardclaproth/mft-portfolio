using MongoDB.Bson;

namespace MyFinancialTracker.Portfolio.Ticker;

public partial class Ticker
{
    public static explicit operator MongoDB.Ticker(Ticker ticker){
        return new MongoDB.Ticker
        {
            Id = ticker.Id == "" || ticker.Id == null ? new ObjectId() : ObjectId.Parse(ticker.Id),
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
