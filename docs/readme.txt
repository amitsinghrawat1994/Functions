Open telemetry setup

OpenTelemetry + Console/Jaeger (Local) → App Insights (Azure)


Install below packages

dotnet add package OpenTelemetry
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http


 Alternative 2: Structured Logging + Seq / Elasticsearch / Loki


MongoDB : mongodb://localhost:27017

Jaeger UI: http://localhost:16686

Seq : http://localhost:5341


 docker-compose up -d

🧹 Clean Up
 docker-compose down
 docker-compose down -v