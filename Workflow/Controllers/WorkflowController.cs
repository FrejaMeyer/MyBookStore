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

        // 1️⃣ Start workflow
        [HttpPost("start-order")]
        public async Task<IActionResult> StartOrder([FromBody] Order order)
        {
            var instanceId = $"book-order-{order.OrderId}";

            try
            {
                _logger.LogInformation("Starting workflow for order {OrderId}", order.OrderId);

                await _daprClient.StartWorkflowAsync(
                    workflowComponent: "bookworkflow",
                    workflowName: nameof(BookOrderingWorkflow),
                    input: order,
                    instanceId: instanceId);

                return Ok(new
                {
                    order_id = order.OrderId,
                    workflow_instance_id = instanceId,
                    status = "started"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start workflow for order {OrderId}", order.OrderId);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // 2️⃣ Ekstern event: BasketValidated
        [Topic("pubsub", "BasketValidated")]
        [HttpPost("events/basket-validated")]
        public async Task<IActionResult> BasketValidated([FromBody] Order result)
        {
            await _daprClient.RaiseWorkflowEventAsync(
                workflowComponent: "bookworkflow",
                instanceId: $"book-order-{result.OrderId}",
                eventName: "BasketValidated",
                eventData: result);
            return Ok();
        }

        // 3️⃣ Ekstern event: OrderCreated
        [Topic("pubsub", "OrderCreated")]
        [HttpPost("events/order-created")]
        public async Task<IActionResult> OrderCreated([FromBody] Order result)
        {
            await _daprClient.RaiseWorkflowEventAsync(
                workflowComponent: "bookworkflow",
                instanceId: $"book-order-{result.OrderId}",
                eventName: "OrderCreated",
                eventData: result);
            return Ok();
        }

        // 4️⃣ Ekstern event: PaymentProcessed
        [Topic("pubsub", "PaymentProcessed")]
        [HttpPost("events/payment-processed")]
        public async Task<IActionResult> PaymentProcessed([FromBody] Order result)
        {
            await _daprClient.RaiseWorkflowEventAsync(
                workflowComponent: "bookworkflow",
                instanceId: $"book-order-{result.OrderId}",
                eventName: "PaymentProcessed",
                eventData: result);
            return Ok();
        }

        [Topic("pubsub", "InventoryChecked")]
        [HttpPost("events/inventory-checked")]
        public async Task<IActionResult> InventoryChecked([FromBody] InventoryResultMessage result)
        {
            await _daprClient.RaiseWorkflowEventAsync(
                workflowComponent: "bookworkflow",
                instanceId: $"book-order-{result.OrderId}",
                eventName: "InventoryChecked",
                eventData: result);
            return Ok();
        }
    }
}
