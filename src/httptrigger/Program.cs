using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using submit_feedback_http_trigger.Extensions;
using submit_feedback_http_trigger.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddMongoDbSettings(Environment.GetEnvironmentVariable("MongoConnectionString"));
builder.Services.AddSingleton<IMongoService, MongoService>();

builder.Build().Run();
