using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace TimerDemo;

public class Function1
{
    private readonly IMongoCollection<QuoteDocument> _quoteCollection;
    private readonly ILogger _logger;
    private readonly HttpClient _client;
    private readonly IMongoClient _mongoClient;

    public Function1(ILoggerFactory loggerFactory,
        IHttpClientFactory httpClientFactory,
        IMongoClient mongoClient)
    {
        _logger = loggerFactory.CreateLogger<Function1>();
        _client = httpClientFactory.CreateClient();
        _mongoClient = mongoClient;

        var database = mongoClient.GetDatabase("MyQuoteDb");
        _quoteCollection = database.GetCollection<QuoteDocument>("quotes");
    }


    [Function("Function1")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation($"C# timmer trigger function executed at: {DateTime.Now}");

        try
        {
            _logger.LogInformation("Fetching new quote from api");

            var response = await _client.GetAsync("https://dummyjson.com/quotes/random");
            response.EnsureSuccessStatusCode();

            var quote = await response.Content.ReadFromJsonAsync<QuoteDocument>();

            if (quote != null)
            {
                await _quoteCollection.InsertOneAsync(quote);
                _logger.LogInformation($"Successfully fetched and saved quote: '{quote.Content}' by {quote.Author}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching or saving the quote.");
        }
    }
}