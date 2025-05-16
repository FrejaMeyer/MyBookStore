using BookOrder.Models;
using Dapr.Client;


namespace BookOrder.Services
{
    public interface IOrderStateService
    {
        Task<Order> UpdateOrderStateAsync(Order order);
        Task<Order?> GetOrderAsync(string orderId);
        Task<string?> DeleteOrderAsync(string orderId);
    }
    public class OrderStateService : IOrderStateService
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<OrderStateService> _logger;

        public OrderStateService(DaprClient daprClient, ILogger<OrderStateService> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        private readonly string _stateStoreName = "bookstatestore";
        public async Task<Order> UpdateOrderStateAsync(Order order)
        {
            try
            {
                var stateKey = $"order_{order.OrderId}";

                // Try to get existing state
                var existingState = await _daprClient.GetStateAsync<Order>(_stateStoreName, stateKey);
                if (existingState != null)
                {
                    // Merge new data with existing state
                    order = MergeOrderStates(existingState, order);
                }

                // Save updated state
                await _daprClient.SaveStateAsync(_stateStoreName, stateKey, order);
                _logger.LogInformation("Updated state for order {OrderId} - Status: {Status}",
                    order.OrderId, order.Status);

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating state for order {OrderId}", order.OrderId);
                throw;
            }
        }

        public async Task<Order?> GetOrderAsync(string orderId)
        {
            try
            {
                // Get order from state store by order ID
                var stateKey = $"order_{orderId}";
                var order = await _daprClient.GetStateAsync<Order>(_stateStoreName, stateKey);

                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    return null;
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<string?> DeleteOrderAsync(string orderId)
        {
            try
            {
                var stateKey = $"order_{orderId}";

                // Tries to delete the order from the state store
                await _daprClient.DeleteStateAsync(_stateStoreName, stateKey);
                _logger.LogInformation("Deleted state for order {OrderId}", orderId);
                return orderId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", orderId);
                throw;
            }
        }

        private Order MergeOrderStates(Order existing, Order update)
        {
            // Preserve important fields from existing state
            update.Customer = update.Customer ?? existing.Customer;
            update.Items = update.Items ?? existing.Items;
            if (update.TotalPrice == 0)
            {
                update.TotalPrice = existing.TotalPrice;
            }

            return update;
        }
    }
}

