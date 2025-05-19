using Dapr.Client;
using Payment.Models;

namespace Payment.Services
{
    public interface IPaymentServices
    {
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest);
        public Task<PaymentResult> GetPaymentStatusAsync(string orderId);
    }


    public class PaymentServices : IPaymentServices
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<PaymentServices> _logger;

        private const string StoreStore = "bookstatestore";
        private const string PaymentTopic = "paymenttopic";
        private const string PubSub = "bookpubsub";

        public PaymentServices(DaprClient daprClient, ILogger<PaymentServices> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            var success = paymentRequest.Amount > 0;

            var result = new PaymentResult
            {
                OrderId = paymentRequest.OrderId,
                Success = success,
                Message = success ? "Payment successful." : "Payment declined."
            };

            _logger.LogInformation(result.OrderId, result.Success, result.Message);

            await _daprClient.SaveStateAsync(StoreStore, paymentRequest.OrderId, result);
            await _daprClient.PublishEventAsync(PubSub, PaymentTopic, result);
            _logger.LogInformation($"Payment processed for OrderId: {paymentRequest.OrderId}, Success: {success}");

            return result;
        }

        public async Task<PaymentResult> GetPaymentStatusAsync(string orderId)
        {
            return await _daprClient.GetStateAsync<PaymentResult>(StoreStore, orderId);
        }
    }
}
