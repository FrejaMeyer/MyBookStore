using Dapr.Client;
using Dapr.Workflow;
using Workflow.Helpers;
using Shared.Messages;
using Workflow.Models;

namespace BookStoreWorkflow.Activities;

public class PaymentActivity : WorkflowActivity<Order, object?>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<PaymentActivity> _logger;

    public PaymentActivity(DaprClient daprClient, ILogger<PaymentActivity> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
    {
        _logger.LogInformation("Processing payment for order {OrderId}", order.OrderId);
        var message = MessageHelper.FillMessage<PaymentMessage>(context, order);
        await _daprClient.PublishEventAsync("bookpubsub", "process-payment", message);
        return null;
    }
}

