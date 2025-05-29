using Dapr.Workflow;
using Shared.Dto;
using Workflow.Models;
using Dapr.Client;


public class InventoryCancelActivity : WorkflowActivity<Order, object?>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<InventoryCancelActivity> _logger;

    public InventoryCancelActivity(DaprClient daprClient, ILogger<InventoryCancelActivity> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
    {
        _logger.LogInformation("Cancelling inventory reservation for order {OrderId}", order.OrderId);

        var cancelMessage = new CancelReservationMessage
        {
            OrderId = order.OrderId,
            Items = order.Items.Select(i => new ReservedItems
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        await _daprClient.InvokeMethodAsync(
            HttpMethod.Post,
            "inventory",
            "inventory/cancel-reservation",
            cancelMessage);

        return null;
    }
}
