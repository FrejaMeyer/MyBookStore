using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages;
using Workflow.Models;
using Workflow.Workflows;

namespace Workflow.Controllers
{
    [ApiController]
    [Route("workflow")]
    public class WorkflowController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<WorkflowController> _logger;

        public WorkflowController(DaprClient daprClient, ILogger<WorkflowController> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

     //   [Topic("bookpubsub", "start-workflow")]
        [HttpPost("start-workflow")]
        public async Task<IActionResult> StartWorkflowViaPubSub([FromBody] BasketMessage message)
        {
            var instanceId = $"book-order-{message.OrderId}";

            await _daprClient.StartWorkflowAsync(
                workflowComponent: "dapr",
                workflowName: nameof(BookOrderingWorkflow),
                input: message,
                instanceId: instanceId);

            _logger.LogInformation("Started workflow via pubsub for {OrderId}", message.OrderId);
            return Ok();
        }

        // 2️⃣ External event: BasketValidated
        [Topic("bookpubsub", "BasketValidated")]
        [HttpPost("events/basket-validated")]
        public async Task<IActionResult> BasketValidated([FromBody] Order result)
        {
            await _daprClient.RaiseWorkflowEventAsync(
                workflowComponent: "dapr",
                instanceId: $"book-order-{result.OrderId}",
                eventName: "BasketValidated",
                eventData: result);
            return Ok();
        }

        // 3️⃣ External event: OrderCreated
        [Topic("bookpubsub", "OrderCreated")]
        [HttpPost("events/order-created")]
        public async Task<IActionResult> OrderCreated([FromBody] Order result)
        {
            await _daprClient.RaiseWorkflowEventAsync(
                workflowComponent: "dapr",
                instanceId: $"book-order-{result.OrderId}",
                eventName: "OrderCreated",
                eventData: result);
            return Ok();
        }

        // 4️⃣ External event: PaymentProcessed
        [Topic("bookpubsub", "PaymentProcessed")]
        [HttpPost("events/payment-processed")]
        public async Task<IActionResult> PaymentProcessed([FromBody] Order result)
        {
            await _daprClient.RaiseWorkflowEventAsync(
                workflowComponent: "dapr",
                instanceId: $"book-order-{result.OrderId}",
                eventName: "PaymentProcessed",
                eventData: result);
            return Ok();
        }

        [Topic("bookpubsub", "InventoryChecked")]
        [HttpPost("events/inventory-checked")]
        public async Task<IActionResult> InventoryChecked([FromBody] InventoryResultMessage result)
        {
            await _daprClient.RaiseWorkflowEventAsync(
                workflowComponent: "dapr",
                instanceId: $"book-order-{result.OrderId}",
                eventName: "InventoryChecked",
                eventData: result);
            return Ok();
        }
    }
}
