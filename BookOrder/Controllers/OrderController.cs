using BookOrder.Models;
using BookOrder.Services;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages;

using Dapr.Client;

namespace BookOrder.Controllers
{
    [ApiController]
    [Route("order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderStateService _orderStateService;
        private readonly ILogger<OrderController> _logger;
        private readonly DaprClient _daprClient;

        public OrderController(IOrderStateService orderStateService, ILogger<OrderController> logger, DaprClient daprClient)
        {
            _orderStateService = orderStateService;
            _logger = logger;
            _daprClient = daprClient;
            
        }

      //  [Topic("bookpubsub", "OrderCreated")]
        [HttpPost("order")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody]Order input)
        {
            _logger.LogInformation("Received new order: {OrderId}", input.OrderId);
            var result = await _orderStateService.UpdateOrderStateAsync(input);
            return CreatedAtAction(nameof(GetOrder), new { orderId = input.OrderId }, result);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrder(string orderId)
        {
            var order = await _orderStateService.GetOrderAsync(orderId);

            if (order == null)
            {
                _logger.LogWarning("Order not found: {OrderId}", orderId);
                return NotFound();
            }

            return Ok(order);
        }




        [HttpDelete("{orderId}")]
        public async Task<ActionResult<string>> DeleteOrder(string orderId)
        {
            var order = await _orderStateService.GetOrderAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            await _orderStateService.DeleteOrderAsync(orderId);
            return Ok(orderId);
        }

       // [Topic("bookpubsub", "OrderCreated")]
        [HttpPost("/orders-sub")]
        public async Task<IActionResult> HandleOrderUpdate(CloudEvent<Order> cloudEvent)
        {
            _logger.LogInformation("Received order update for order {OrderId}",
                cloudEvent.Data.OrderId);

            var result = await _orderStateService.UpdateOrderStateAsync(cloudEvent.Data);
            return Ok();
        }


    }
}

        //[Topic("bookpubsub", "validate-basket")]
        //[HttpPost("/validate-basket")]
        //public async Task<IActionResult> ValidateBasket([FromBody] BasketMessage message)
        //{
        
        //    _logger.LogInformation("Validating basket for workflow {workflowId}", message.WorkflowId);

        //    var result = new Order
        //    {
        //        OrderId = message.OrderId,
        //        Customer = new Customer
        //        {
        //            CustomerId = message.Customer.CustomerId,
        //            Name = message.Customer.Name,
        //            Email = message.Customer.Email,
        //            Address = message.Customer.Address
        //        },
        //        Items = message.Items.Select(i => new OrderItem
        //        {
        //            ProductId = i.ProductId,
        //            Name = i.Name,
        //            Quantity = i.Quantity,
        //            UnitPrice = i.UnitPrice
        //        }).ToList(),
        //        TotalPrice = message.TotalPrice,
        //        Status = OrderStatus.Validated,
        //    };

        //    await _orderStateService.UpdateOrderStateAsync(result);

        //    await _daprClient.PublishEventAsync("bookpubsub", "OrderCreated", result);

        //    _logger.LogInformation("Ordre saved and event senst: {OrderId}", result.OrderId);

        //    return Ok();
        //}