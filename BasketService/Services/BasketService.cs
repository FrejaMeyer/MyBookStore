using Basket.Models;
using Dapr.Client;

namespace Basket.Services
{
    public interface IBasketService
    {
        public Task<Cart> GetCartAsync(string customerId);
        public Task AddItemAsync(string customerId, CartItems item);
        public Task RemoveItemAsync(string customerId, string productId);
        public Task UpdateCartAsync(string customerId, Cart cart);
    }

    public class BasketService : IBasketService
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<BasketService> _logger;
        private readonly string _statestore = "bookstatestore";
        private readonly string _pubsubName = "bookpubsub";
        private readonly string _topicName = "checkout-request";

        public BasketService(DaprClient daprClient, ILogger<BasketService> iLogger)
        {
            _daprClient = daprClient;
            _logger = iLogger;
        }

        public async Task<Cart> GetCartAsync(string customerId)
        {
            _logger.LogInformation("Getting cart for customer {customerId}", customerId);
            var cart = await _daprClient.GetStateAsync<Cart>(_statestore, customerId);
            return cart ?? new Cart
            {
                Customer = new Customer { CustomerId = customerId },
                Items = new List<CartItems>()
            };
        }

        public async Task UpdateCartAsync(string customerId, Cart cart)
        {
            await _daprClient.SaveStateAsync(_statestore, customerId, cart);
            _logger.LogInformation("Cart updated for customer {customerId} with {ItemCount} items", customerId, cart.Items.Count);
        }

        public async Task AddItemAsync(string customerId, CartItems newItem)
        {
            var cart = await GetCartAsync(customerId);
            var newId = newItem.ProductId?.Trim().ToLowerInvariant();


            var existingItem = cart.Items.FirstOrDefault(i =>
                i.ProductId?.Trim().ToLowerInvariant() == newId);

            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
                _logger.LogInformation("Updated quantity for {ProductId}: {Quantity}", newItem.ProductId, existingItem.Quantity);
            }
            else
            {
                cart.Items.Add(newItem);
                _logger.LogInformation("Added item {ProductId} to cart for customer {customerId}", newItem.ProductId, customerId);
            }

            await _daprClient.SaveStateAsync(_statestore, customerId, cart);
        }


        public async Task RemoveItemAsync(string customerId, string productId)
        {
            var cart = await GetCartAsync(customerId);
            var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
                await _daprClient.SaveStateAsync(_statestore, customerId, cart);
                _logger.LogInformation("Removed item {productId} from cart for customer {customerId}", productId, customerId);
            }
        }
    }
}
