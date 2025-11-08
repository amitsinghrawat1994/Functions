using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using submit_feedback_http_trigger.Extensions;
using submit_feedback_http_trigger.Services;

var builder = FunctionsApplication.CreateBuilder(args);

// Use environment to decide exporter
var exporter = Environment.GetEnvironmentVariable("TELEMETRY_EXPORTER") ?? "console";


builder.ConfigureFunctionsWebApplication();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: "FeedbackApi"))
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation() // http triggers
            .AddHttpClientInstrumentation() // Outgoing HTTP calls
            .SetSampler(new AlwaysOnSampler());

        if (exporter.ToLower() == "console")
        {
            tracerProviderBuilder.AddConsoleExporter();
        }
        else if (exporter.ToLower() == "jaeger")
        {
            tracerProviderBuilder.AddJaegerExporter();
        }
        // else if (exporter == "appinsights")
        // {
        //     var connString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
        //     if (!string.IsNullOrEmpty(connString))
        //     {
        //         tracerProviderBuilder.AddAzureMonitorTraceExporter(options =>
        //         {
        //             options.ConnectionString = connString;
        //         });
        //     }
        // }
        else
        {
            tracerProviderBuilder.AddConsoleExporter();
        }
    });

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddMongoDbSettings(Environment.GetEnvironmentVariable("MongoConnectionString"));
builder.Services.AddSingleton<IMongoService, MongoService>();

builder.Build().Run();
