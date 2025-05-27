using Basket.Services;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Shared.Messages;
using Basket.Models;

[ApiController]
[Route("basket")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly ILogger<BasketController> _logger;
    private readonly DaprClient _daprClient;
    private readonly HttpClient _http;

    public BasketController(IBasketService basketService, ILogger<BasketController> logger, DaprClient daprClient)
    {
        _basketService = basketService;
        _logger = logger;
        _daprClient = daprClient;
    }

    //Hent kurv – bruger customerId fra URL
    [HttpGet("{customerId}")]
    public async Task<ActionResult<Cart>> GetCart(string customerId)
    {
        var cart = await _basketService.GetCartAsync(customerId);
        return cart == null ? NotFound() : Ok(cart);
    }

    //Læg i kurv – bruger customerId fra URL
    [HttpPost("{customerId}/items")]
    public async Task<IActionResult> AddItem(string customerId, [FromBody] CartItemDto item)
    {
        var model = new CartItems
        {
            ProductId = item.ProductId,
            Name = item.Name,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        };

        await _basketService.AddItemAsync(customerId, model);
        return Ok();
    }

    //Fjern fra kurv – bruger customerId og productId fra URL
    [HttpDelete("{customerId}/items/{productId}")]
    public async Task<IActionResult> RemoveItem(string customerId, string productId)
    {
        await _basketService.RemoveItemAsync(customerId, productId);
        return Ok();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CustomerDto customerDto)
    {
        var cart = await _basketService.GetCartAsync(customerDto.CustomerId);

        if (cart == null || cart.Items == null || !cart.Items.Any())
        {
            return BadRequest("Cart is empty.");
        }

        if (cart.Customer == null)
        {
            cart.Customer = new Customer
            {
                CustomerId = customerDto.CustomerId,
                Name = customerDto.Name,
                Email = customerDto.Email,
                Address = customerDto.Address
            };
        }

        var orderId = Guid.NewGuid().ToString();

        var message = new BasketMessage
        {
            OrderId = orderId,
            Customer = customerDto,
            Items = cart.Items.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                Name = item.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList(),
            TotalPrice = cart.Items.Sum(i => i.Quantity * i.UnitPrice),
            WorkflowId = $"wf-{Guid.NewGuid()}"
        };

          await _daprClient.PublishEventAsync("bookpubsub", "start-workflow", message);
       // await _http.PostAsJsonAsync("workflow/start-workflow", message);

        _logger.LogInformation(" Sends BasketMessage: OrderId={OrderId}, Items={ItemCount}, Customer={Customer}",
    message.OrderId, message.Items.Count, message.Customer.Name);

        await _daprClient.PublishEventAsync("bookpubsub", "validate-basket", message);

        return Ok(new { orderId });
    }

    [HttpPut("{customerId}/items")]
    public async Task<IActionResult> UpdateItem(string customerId, [FromBody] CartItemDto item)
    {
        var cart = await _basketService.GetCartAsync(customerId);
        if (cart == null)
        {
            return NotFound("Cart not found.");
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem == null)
        {
            return NotFound("Item not found in cart.");
        }

        existingItem.Quantity = item.Quantity;
        await _basketService.UpdateCartAsync(customerId, cart);

        return Ok();
    }
}
