using Dapr.Client;
using Dapr.Workflow;
using Workflow.Helpers;
using Shared.Messages;
using Workflow.Models;

namespace Workflow.Activites
{
    public class OrderActivity : WorkflowActivity<Order, object?>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<OrderActivity> _logger;

        public OrderActivity(DaprClient daprClient, ILogger<OrderActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }
        public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
        {
            _logger.LogInformation("Creating order {OrderId}", order.OrderId);
            var message = MessageHelper.FillMessage<OrderMessage>(context, order);
            await _daprClient.PublishEventAsync("bookpubsub", "create-order", message);
            return null;
        }
    }
}

