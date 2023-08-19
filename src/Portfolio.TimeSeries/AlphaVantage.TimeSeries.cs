using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace LClaproth.MyFinancialTracker.Portfolio.AlphaVantage;

public class TimeSeries
{
    [JsonPropertyName("Meta Data")]
    public MetaData MetaData { get; set; }

    [JsonPropertyName("Time Series (Daily)")]
    public Dictionary<string, Daily> Dailies { get; set; }

    public static explicit operator MongoDB.TimeSeries(TimeSeries timeSeries){
        return new MongoDB.TimeSeries {
            Id = new ObjectId(),
            Symbol = timeSeries.MetaData.Symbol,
            LastRefreshed = timeSeries.MetaData.LastRefreshed,
            Dailies = timeSeries.Dailies
        };
    }
}

public class MetaData {
    [JsonPropertyName("2. Symbol")]
    public string Symbol { get; set; }
    [JsonPropertyName("3. Last Refreshed")]
    public DateTimeOffset LastRefreshed { get; set; }
}

public class Daily
{
    [JsonPropertyName("1. open")]
    public double Open { get; set; }
    [JsonPropertyName("2. high")]
    public double High { get; set; }
    [JsonPropertyName("3. low")]
    public double Low { get; set; }
    [JsonPropertyName("4. close")]
    public double Close { get; set; }
    [JsonPropertyName("5. volume")]
    public double Volume { get; set; }
}