using Basket.Models;
using Dapr.Client;

namespace Basket.Services
{
    public interface IBasketService
    {
        public Task<Cart> GetCartAsync(string customerId);
        public Task AddItemAsync(string customerId, CartItems item);
        public Task RemoveItemAsync(string customerId, string productId);
      //  public Task Checkout(string customerId);
    }

    public class BasketService : IBasketService
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<BasketService> _logger;
        private readonly string _stateStoreName = "bookstatestore";
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
            var cart = await _daprClient.GetStateAsync<Cart>(_stateStoreName, customerId);
            return cart ?? new Cart
            {
                Customer = new Customer { CustomerId = customerId },
                Items = new List<CartItems>()
            };
        }
         
        public async Task AddItemAsync(string customerId, CartItems item)
        {
            var cart = await GetCartAsync(customerId);
            cart.Items.Add(item);
            await _daprClient.SaveStateAsync(_stateStoreName, customerId, cart);
            _logger.LogInformation("Added item {item.ProductId} to cart for customer {customerId}", item.ProductId, customerId);
        }

        public async Task RemoveItemAsync(string customerId, string productId)
        {
            var cart = await GetCartAsync(customerId);
            var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
                await _daprClient.SaveStateAsync(_stateStoreName, customerId, cart);
                _logger.LogInformation("Removed item {productId} from cart for customer {customerId}", productId, customerId);
            }   
        }

        //public async Task Checkout(string customerId)
        //{
        //    var cart = await GetCartAsync(customerId);
        //    _logger.LogInformation("Checkout initiated for customer {customerId}", customerId);
        //    await _daprClient.PublishEventAsync(_pubsubName, _topicName, cart);
        //}








            //    public async Task AddItemAsync(string customerId, CartItems item)
            //    {
            //        _logger.LogInformation("Adding {item.ProductId} x{Quantity} to cart for customer {customerId}", item.ProductId, item.Quantity, customerId);

            //        try
            //        {
            //            var cart = await GetCartAsync(customerId);

            //            var existing = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            //            if (existing != null)
            //            {
            //                existing.Quantity += item.Quantity;
            //                _logger.LogInformation("Updated Quantity: {ProductId} x{Quantity}", item.ProductId, existing.Quantity);
            //            }
            //            else
            //            {
            //                cart.Items.Add(item);
            //                _logger.LogInformation("Added new item: {ProductId} x{Quantity}", item.ProductId, item.Quantity);
            //            }

            //            await _daprClient.SaveStateAsync(_stateStoreName, customerId, cart);
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, "Error adding item to cart for customer {customerId}", customerId);
            //            throw;
            //        }
            //    }

            //    public async Task<Cart> GetCartAsync(string customerId)
            //    {
            //        _logger.LogInformation("Getting cart for customer {customerId}", customerId);
            //        try
            //        {
            //            var basket = await _daprClient.GetStateAsync<Cart>(_stateStoreName, customerId);
            //            if (basket == null)
            //            {
            //                _logger.LogInformation("No cart found for customer {customerId}", customerId);
            //                return new Cart
            //                {
            //                    Customer = new Customer { CustomerId = customerId },
            //                    Items = new List<CartItems>()
            //                };
            //            }
            //            _logger.LogInformation("Cart found for customer {customerId}", customerId);
            //            return basket;
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, "Error retrieving cart for customer {customerId}", customerId);
            //            throw;
            //        }
            //    }

            //    public async Task RemoveItemAsync(string customerId, string productId)
            //    {
            //        _logger.LogInformation("Removing {productId} from cart for customer {customerId}", productId, customerId);

            //        try
            //        {
            //            var cart = await GetCartAsync(customerId);
            //            cart.Items.RemoveAll(i => i.ProductId == productId);

            //            await _daprClient.SaveStateAsync(_stateStoreName, customerId, cart);
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, "Error removing item from cart for customer {customerId}", customerId);
            //            throw;
            //        }
            //    }
        }
}
