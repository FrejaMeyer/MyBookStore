using Dapr.Workflow;
using Shared.Dto;
using Workflow.Models;
using Dapr.Client;

namespace Workflow.Activites
{
    public class InventoryReserveActivity : WorkflowActivity<Order, Order>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<InventoryReserveActivity> _logger;

        public InventoryReserveActivity(DaprClient daprClient, ILogger<InventoryReserveActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<Order> RunAsync(WorkflowActivityContext context, Order order)
        {
            _logger.LogInformation("Reserving stock for order {OrderId}", order.OrderId);

            var message = new ReserveStockMessage
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
                "inventory/reserve",
                message);

            return order;
        }
    }

}
