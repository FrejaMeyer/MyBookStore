using Microsoft.AspNetCore.Mvc;
using Inventory.Services;
using Inventory.Models;
using Dapr.Client;
using Shared.Messages;
using Dapr;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;
        private readonly DaprClient _daprClient; 

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger, DaprClient daprClient)
        {
            _inventoryService = inventoryService;
            _logger = logger;
            _daprClient = daprClient;
        }


        // Get a single inventory item
        [HttpGet("{productId}")]
        public async Task<ActionResult<InventoryItem>> GetItemAsync(string productId)
        {
            var item = await _inventoryService.GetItemAsync(productId);
            return Ok(item);
        }

        //Add or update inventory
        [HttpPost]
        public async Task<IActionResult> SetItemAsync([FromBody] InventoryItem item)
        {
            await _inventoryService.SetItemAsync(item);
            return NoContent();
        }

        // Reserve stock manually 
        [HttpPost("reserve/{productId}")]
        public async Task<IActionResult> ReserveStockAsync(string productId, int quantity)
        {
            var success = await _inventoryService.ReserveStockAsync(productId, quantity);
            if (success)
            {
                return Ok(success);
            }
            else
            {
                return BadRequest("Not enough stock available.");
            }
        }

        // Handle inventory check from workflow
        [Topic("bookpubsub", "check-inventory")]
        [HttpPost("/check-inventory")]
        public async Task<IActionResult> CheckInventory([FromBody] InventoryMessage message)
        {
            _logger.LogInformation("Checking inventory for Order {OrderId} / Workflow {WorkflowId}", message.OrderId, message.WorkflowId);

            var success = await _inventoryService.ReserveStockAsync(message.ProductId, message.Quantity);

            var result = new InventoryResultMessage
            {
                WorkflowId = message.WorkflowId,
                OrderId = message.OrderId,
                ProductId = message.ProductId,
                Quantity = message.Quantity,
                Status = success ? "reserved" : "failed",
                Error = success ? null : "Not enough stock"
            };

            await _daprClient.PublishEventAsync("bookpubsub", "InventoryChecked", result);

            return Ok();
        }
    }
}
