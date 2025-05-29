using Dapr.Workflow;
using Shared.Dto;
using Dapr.Client;
using Workflow.Models;
using Shared.Messages;

namespace Workflow.Activites
{
    public class InventoryConfirmActivity : WorkflowActivity<Order, object?>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<InventoryConfirmActivity> _logger;

        public InventoryConfirmActivity(DaprClient daprClient, ILogger<InventoryConfirmActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
        {
            _logger.LogInformation("Confirming inventory for order {OrderId}", order.OrderId);

            var message = new ConfirmReservationMessage
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
                        "inventory/confirm-reservation",
                        message);

            return null;
        }
    }

}
