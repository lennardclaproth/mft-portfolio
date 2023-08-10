using MongoDB.Bson;
using MyFinancialTracker.Portfolio.AlphaVantage;

namespace MyFinancialTracker.Portfolio.MongoDB;

[BsonCollection("time_series")]
public class TimeSeries : IDocument
{
    public ObjectId Id { get; set; }
    public string Symbol { get; set; }
    public DateTimeOffset LastRefreshed { get; set; }
    public Dictionary<string, Daily> Dailies { get; set; }
    public DateTime CreatedAt => throw new NotImplementedException();
}
