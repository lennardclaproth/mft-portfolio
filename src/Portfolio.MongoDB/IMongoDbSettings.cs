namespace MyFinancialTracker.Portfolio.MongoDB;

public interface IMongoDbSettings
{
    string Database { get; set; }
    string ConnectionString { get; set; }
}
