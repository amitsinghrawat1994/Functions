using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Shared;

namespace CreateOrder;

public class CreateOrder
{
    private readonly ILogger<CreateOrder> _logger;

    public CreateOrder(ILogger<CreateOrder> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public async Task<MultiOutput> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var newOrder = await req.ReadFromJsonAsync<OrderRequest>();
        if (newOrder == null || newOrder.CustomerId <= 0)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid order data.");
            return new MultiOutput { HttpResponse = badRequest };
        }

        newOrder.OrderId = Guid.NewGuid().ToString();
        newOrder.Status = "Pending";

        _logger.LogInformation($"Order {newOrder.OrderId} received. Submitting to processing queue.");

        var response = req.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteAsJsonAsync(new { newOrder.OrderId, Status = "Order submitted for processing." });
        return new MultiOutput
        {
            HttpResponse = response,
            OrderMessage = newOrder
        };
    }

    public class MultiOutput
    {
        // This attribute sends the "OrderMessage" property to the queue
        [QueueOutput("orders-to-process", Connection = "AzureWebJobsStorage")]
        public OrderRequest OrderMessage { get; set; }

        public HttpResponseData HttpResponse { get; set; }
    }
}