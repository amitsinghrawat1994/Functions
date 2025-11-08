using System;
using Microsoft.Extensions.DependencyInjection;
using submit_feedback_http_trigger.Models;

namespace submit_feedback_http_trigger.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbSettings(this IServiceCollection services, string connectionString)
    {
        services.Configure<MongoDbSettings>(options =>
        {
            options.ConnectionString = connectionString;
            options.DatabaseName = "FeedbackDb";
            options.CollectionName = "Feedback";
        });

        return services;
    }

}
