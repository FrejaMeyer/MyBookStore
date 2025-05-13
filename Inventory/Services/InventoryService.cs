using Inventory.Models;
using Dapr.Client;
using System.Net.WebSockets;

namespace Inventory.Services
{
    public interface IInventoryService
    {
        public Task<InventoryItem> GetItemAsync(string productId);
        public Task SetItemAsync(InventoryItem item);
        public Task<bool> ReserveStockAsync(string productId, int quantity);
    }
    public class InventoryService : IInventoryService
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<InventoryService> _logger;

        private const string StoreName = "bookstatestore";
        private const string Pubsub = "bookpubsub";
        private const string TopicName = "inventorytopic";

        public InventoryService(DaprClient daprclient, ILogger<InventoryService> logger)
        {
            _daprClient = daprclient;
            _logger = logger;
        }

        public async Task<InventoryItem> GetItemAsync(string productId)
        {
            var item = await _daprClient.GetStateAsync<InventoryItem>(StoreName, productId);
            return item ?? new InventoryItem
            {
                ProductId = productId,
                ProductName = "Unknown",
                Instock = false,
                QuantityAvailable = 0
            };
        }

        public async Task SetItemAsync(InventoryItem item)
        {
            await _daprClient.SaveStateAsync(StoreName, item.ProductId, item);
            _logger.LogInformation("Inventory updated for {ProductId}", item.ProductId);
        }
        public async Task<bool> ReserveStockAsync(string productId, int quantity)
        {
            var item = await GetItemAsync(productId);
            if (item.QuantityAvailable < quantity)
            {
                _logger.LogWarning("Not enough stock for {ProductId}. Available: {Available}, Requested: {Requested}", productId, item.QuantityAvailable, quantity);
                return false;
            }
            item.QuantityAvailable -= quantity;
            await SetItemAsync(item);
            _logger.LogInformation("Reserved {Quantity} of {ProductId}. Remaining: {Remaining}", quantity, productId, item.QuantityAvailable);
            await _daprClient.PublishEventAsync(Pubsub, TopicName, item);
            return true;
        }

    }
}
