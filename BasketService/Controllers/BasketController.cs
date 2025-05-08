using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Basket.Services;
using Basket.Models;

namespace Basket.Controllers
{
    [ApiController]
    [Route("basket/{customerId}")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly ILogger<BasketController> _logger;
        private readonly DaprClient _daprClient;

        //private const string PUBSUB_NAME = "bookpubsub";
        //private const string TOPIC_NAME = "basket";

        public BasketController(IBasketService basketService, ILogger<BasketController> logger, DaprClient daprClient)
        {
            _basketService = basketService;
            _logger = logger;
            _daprClient = daprClient;
        }

        [HttpGet]
        public async Task<ActionResult<Cart>> GetCart(string customerId)
        {
            //_logger.LogInformation("Getting cart for customer {customerId}", customerId);
            var cart = await _basketService.GetCartAsync(customerId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<Cart>> AddItem(string customerId, [FromBody] CartItems item)
        {
            //_logger.LogInformation("Adding {item.ProductId} x{Quantity} to cart for customer {customerId}", item.PruductId, item.Quantity, customerId);
            await _basketService.AddItemAsync(customerId, item);
            return Ok();
        }

        [HttpDelete("items/{productId}")]
        public async Task<ActionResult<Cart>> RemoveItem(string customerId, string productId)
        {
            //_logger.LogInformation("Removing {productId} from cart for customer {customerId}", productId, customerId);
            await _basketService.RemoveItemAsync(customerId, productId);
            return Ok();
        }

        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout(string customerId)
        {
            //_logger.LogInformation("Checking out cart for customer {customerId}", customerId);
           await _basketService.Checkout(customerId);
            //await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, customerId);
            return Ok();
        }
    }
}
