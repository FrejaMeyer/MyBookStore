using Dapr.Client;
using Dapr.Workflow;
using Workflow.Helpers;
using Shared.Messages;
using Workflow.Models;
namespace Workflow.Activites
{
    public class BasketActivity : WorkflowActivity<Order, object?>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<BasketActivity> _logger;

        public BasketActivity(DaprClient daprClient, ILogger<BasketActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }
        public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
        {
            _logger.LogInformation("Validating basket for order {OrderId}", order.OrderId);
            var message = MessageHelper.FillMessage<BasketMessage>(context, order);
            await _daprClient.PublishEventAsync("bookpubsub", "validate-basket", message);
            return null;
        }
    }
}
