using Dapr.Client;
using Dapr.Workflow;
using Workflow.Models;

namespace Workflow.Activites
{
    public class OrderActivity : WorkflowActivity<Order, Order>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<OrderActivity> _logger;
        public OrderActivity(DaprClient daprClient, ILogger<OrderActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<Order> RunAsync(WorkflowActivityContext context, Order order)
        {
            try
            {
                _logger.LogInformation("Creating order {OrderId}", order.OrderId);

                var response = await _daprClient.InvokeMethodAsync<Order, Order>(
                    HttpMethod.Post,
                    "bookOrder",     // Navnet på mikrotjenesten der håndterer ordreoprettelse
                    "create",            // Endpoint i tjenesten
                    order);              // Payload

                _logger.LogInformation("Order {OrderId} created successfully with status {Status}",
                    response.OrderId, response.Status);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order {OrderId}", order.OrderId);
                throw;
            }
        }

        //public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
        //{
        //    _logger.LogInformation("Creating order {OrderId}", order.OrderId);
        //    var message = MessageHelper.FillMessage<OrderMessage>(context, order);
        //    await _daprClient.PublishEventAsync("bookpubsub", "create-order", message);
        //    return null;
        //}
    }
}

