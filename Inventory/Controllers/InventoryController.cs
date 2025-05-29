using Microsoft.AspNetCore.Mvc;
using Inventory.Services;
using Inventory.Models;
using Dapr.Client;
using Shared.Messages;
using Shared.Dto;
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
        public async Task<IActionResult> SetItemAsync([FromBody] InventoryItemDto item)
        {
            await _inventoryService.SetItemAsync(item);
            return NoContent();
        }

        // Reserve stock manually 
        [HttpPost("reserve/{productId}")]
        public async Task<IActionResult> ReserveStockAsync(string productId, int quantity, string orderId)
        {
            var success = await _inventoryService.ReserveStockAsync(productId, quantity, orderId);
            if (success)
            {
                return Ok(success);
            }
            else
            {
                return BadRequest("Not enough stock available.");
            }
        }

        // Confirmand remove from storage
        [HttpPost("confirm-reservation")]
        public async Task<IActionResult> ConfirmReservation([FromBody] ConfirmReservationMessage message)
        {
            foreach (var item in message.Items)
            {
                var stock = await _inventoryService.GetItemAsync(item.ProductId);
                stock.QuantityAvailable -= item.Quantity;
               // stock.QuantityReserved -= item.Quantity;
                await _inventoryService.SetItemAsync(stock);
            }

            return Ok();
        }

        // Handle inventory check from workflow
        [Topic("bookpubsub", "check-inventory")]
        [HttpPost("/check-inventory")]
        public async Task<IActionResult> CheckInventory([FromBody] InventoryMessage message)
        {
            _logger.LogInformation("Checking inventory for Order {OrderId} / Workflow {WorkflowId}", message.OrderId, message.WorkflowId);

            var success = await _inventoryService.ReserveStockAsync(message.ProductId, message.Quantity, message.OrderId);

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

        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve([FromBody] ReserveStockMessage message)
        {
            foreach (var item in message.Items)
            {
                var stock = await _inventoryService.GetItemAsync(item.ProductId);

                if (stock.QuantityAvailable < item.Quantity)
                    return BadRequest($"Not enough stock for {item.ProductId}");

                stock.QuantityAvailable -= item.Quantity;
                stock.QuantityReserved += item.Quantity;

                await _inventoryService.SetItemAsync(stock);
            }

            return Ok();
        }

        [HttpPost("cancel-reservation")]
        public async Task<IActionResult> CancelReservation([FromBody] CancelReservationMessage message)
        {
            foreach (var item in message.Items)
            {
                var stock = await _inventoryService.GetItemAsync(item.ProductId);

                stock.QuantityReserved -= item.Quantity;
                stock.QuantityAvailable += item.Quantity;

                await _inventoryService.SetItemAsync(stock);
            }

            return Ok();
        }


    }
}
