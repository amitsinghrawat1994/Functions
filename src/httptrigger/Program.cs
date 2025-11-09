using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using submit_feedback_http_trigger.Extensions;
using submit_feedback_http_trigger.Services;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Worker", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    // .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341") // Seq URL
    .CreateLogger();

try
{



    var builder = FunctionsApplication.CreateBuilder(args);

    // Use environment to decide exporter
    var exporter = Environment.GetEnvironmentVariable("TELEMETRY_EXPORTER") ?? "console";

    // Use Serilog as the logging provider
    builder.Services.AddSerilog(Log.Logger);

    builder.ConfigureFunctionsWebApplication();

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(serviceName: "FeedbackApi"))
        .WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddAspNetCoreInstrumentation() // http triggers
                .AddHttpClientInstrumentation() // Outgoing HTTP calls
                .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:4317"); // OTLP gRPC
                    })
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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}