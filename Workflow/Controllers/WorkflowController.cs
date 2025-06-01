using Dapr;
using Dapr.Client;
using Dapr.Workflow;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages;
using Shared.Dto;
using Workflow.Models;
using Workflow.Workflows;

namespace Workflow.Controllers
{
    [ApiController]
    [Route("workflow")]
    public class WorkflowController : ControllerBase
    {
        private readonly DaprWorkflowClient _daprClient;
        private readonly ILogger<WorkflowController> _logger;

        public WorkflowController(DaprWorkflowClient daprClient, ILogger<WorkflowController> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }


        [HttpPost("start-order")]
        public async Task<IActionResult> StartOrder([FromBody] OrderDto order)
        {
            var instanceId = $"book-order-{order.OrderId}";

            try
            {
                _logger.LogInformation("Starting workflow for order {OrderId}", order.OrderId);

                await _daprClient.ScheduleNewWorkflowAsync(
                    nameof(BookOrderingWorkflow),
                    instanceId,
                    order
                );

                _logger.LogInformation("Workflow started successfully for order {OrderId}", order.OrderId);

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
                return StatusCode(500, new
                {
                    order_id = order.OrderId,
                    error = ex.Message
                });
            }
        }
    }
}
     

    









//    //   [Topic("bookpubsub", "start-workflow")]
//    [HttpPost("start-workflow")]
//        public async Task<IActionResult> StartWorkflowViaPubSub([FromBody] BasketMessage message)
//        {
//            var instanceId = $"book-order-{message.OrderId}";

//            await _daprClient.StartWorkflowAsync(
//                workflowComponent: "dapr",
//                workflowName: nameof(BookOrderingWorkflow),
//                input: message,
//                instanceId: instanceId);

//            _logger.LogInformation("Started workflow via pubsub for {OrderId}", message.OrderId);
//            return Ok();
//        }

//        [Topic("bookpubsub", "BasketValidated")]
//        [HttpPost("events/basket-validated")]
//        public async Task<IActionResult> BasketValidated([FromBody] Order result)
//        {
//            await _daprClient.RaiseWorkflowEventAsync(
//                workflowComponent: "dapr",
//                instanceId: $"book-order-{result.OrderId}",
//                eventName: "BasketValidated",
//                eventData: result);
//            return Ok();
//        }

//        [Topic("bookpubsub", "OrderCreated")]
//        [HttpPost("events/order-created")]
//        public async Task<IActionResult> OrderCreated([FromBody] Order result)
//        {
//            await _daprClient.RaiseWorkflowEventAsync(
//                workflowComponent: "dapr",
//                instanceId: $"book-order-{result.OrderId}",
//                eventName: "OrderCreated",
//                eventData: result);
//            return Ok();
//        }

//        [Topic("bookpubsub", "PaymentProcessed")]
//        [HttpPost("events/payment-processed")]
//        public async Task<IActionResult> PaymentProcessed([FromBody] Order result)
//        {
//            await _daprClient.RaiseWorkflowEventAsync(
//                workflowComponent: "dapr",
//                instanceId: $"book-order-{result.OrderId}",
//                eventName: "PaymentProcessed",
//                eventData: result);
//            return Ok();
//        }

//        [Topic("bookpubsub", "InventoryChecked")]
//        [HttpPost("events/inventory-checked")]
//        public async Task<IActionResult> InventoryChecked([FromBody] InventoryResultMessage result)
//        {
//            await _daprClient.RaiseWorkflowEventAsync(
//                workflowComponent: "dapr",
//                instanceId: $"book-order-{result.OrderId}",
//                eventName: "InventoryChecked",
//                eventData: result);
//            return Ok();
//        }
//    }
//}
