namespace LClaproth.MyFinancialTracker.Portfolio.AlphaVantage;

public static class QueryBuilder
{
    public static string Build(ApiFunction apiFunction, Dictionary<string, string> requestParameters, string apiKey){
        string queryString = $"?function={apiFunction}";
        foreach (var parameter in requestParameters)
        {
            queryString += $"&{parameter.Key}={parameter.Value}";
        }
        queryString += $"&apikey={apiKey}";
        return queryString;
    }
}
