using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using LClaproth.MyFinancialTracker.Portfolio.Ticker;
using LClaproth.MyFinancialTracker.Portfolio.MongoDB;
using LClaproth.MyFinancialTracker.Portfolio.TimeSeries;
using LClaproth.MyFinancialTracker.Portfolio.AlphaVantage;
using LClaproth.MyFinancialTracker.EventBus;
using LClaproth.MyFinancialTracker.EventBus.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

var ApplicationName = builder.Configuration.GetValue<string>("ApplicationInfo:ApplicationName") ?? throw new InvalidOperationException("Application name 'ApplicationInfo:ApplicationName' not found.");
Environment.SetEnvironmentVariable("ApplicationName",ApplicationName);
// var env = Environment.GetEnvironmentVariables();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<IMongoDbSettings>(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.Configure<AlphaVantageConfiguration>(builder.Configuration.GetSection("AlphaVantage"));
builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

builder.Services.AddControllers().AddJsonOptions(
    options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRabbitMQEventBus();

// builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddSingleton<RequestHandler>();

builder.Services.AddTransient<TimeSeriesService>();
builder.Services.AddTransient<TickerService>();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TickerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapGrpcReflectionService();
app.MapControllers();

app.Run();
