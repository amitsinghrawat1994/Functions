using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddHttpClient()
    .AddSingleton<IMongoClient>(s =>
    {
        var connectionString = Environment.GetEnvironmentVariable("MongoDbConnection");
        return new MongoClient(connectionString);
    });

builder.Build().Run();
