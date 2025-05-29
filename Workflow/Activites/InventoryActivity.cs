using Dapr.Client;
using Microsoft.Extensions.Logging;
using Dapr.Workflow;
using Workflow.Models; 

namespace Workflow.Activites
{
    public class InventoryActivity : WorkflowActivity<Order, Order>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<InventoryActivity> _logger;

        public InventoryActivity(DaprClient daprClient, ILogger<InventoryActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<Order> RunAsync(WorkflowActivityContext context, Order order)
        {
            try
            {
                _logger.LogInformation("Updating inventory for order {OrderId}", order.OrderId);

                // Kald InventoryService og opdater lageret
                var response = await _daprClient.InvokeMethodAsync<Order, Order>(
                    HttpMethod.Post,
                    "inventory-service",         // navnet på din InventoryService (dapr app-id)
                    "update-stock",              // endpoint i InventoryController
                    order);                      // sender hele ordren (du kan også sende kun Items)

                if (response == null || response.Status != "stock_updated")
                {
                    throw new Exception("Inventory update failed or returned invalid response.");
                }

                _logger.LogInformation("Inventory updated for order {OrderId}", order.OrderId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Inventory update failed for order {OrderId}", order.OrderId);
                order.Status = "failed";
                order.Error = ex.Message;
                return order;
            }
        }
    }
}
