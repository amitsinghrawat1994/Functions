using System;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared;

namespace ProcessOrder;

public class ProcessOrder
{
    private readonly ILogger<ProcessOrder> _logger;

    public ProcessOrder(ILogger<ProcessOrder> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ProcessOrder))]
    public async Task Run([QueueTrigger("orders-to-process", Connection = "StorageConnection")] OrderRequest order)
    {
        _logger.LogInformation($"Processing order {order.OrderId} for customer {order.CustomerId}");

        try
        {
            await Task.Delay(1000);
            _logger.LogInformation($"Payment processed for {order.OrderId}");

            await Task.Delay(1000);
            _logger.LogInformation($"Confirmation email sent for {order.OrderId}.");

            _logger.LogInformation($"Order {order.OrderId} processed successfully")
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to process order {order.OrderId}");
            throw;
        }
    }
}