using Shared.Dto;
using System.IO.Pipes;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly CustomerSessionService _session;

        public PaymentService(HttpClient httpClient, CustomerSessionService session)
        {
            _httpClient = httpClient;
            _session = session;
        }

        public async Task<bool> SimulatePaymentAsync(PaymentRequestDto request, OrderDto order)
        {
            request.CustomerId = _session.GetCustomerId();

            // 1. Opret ordren via Dapr (POST)
            var orderResponse = await _httpClient.PostAsJsonAsync(
                "orderservice/method/order/order", order);

            if (!orderResponse.IsSuccessStatusCode)
            {
                // Log evt. fejl
                return false;
            }

            // 2. Send betaling via Dapr (POST)
            var paymentResponse = await _httpClient.PostAsJsonAsync(
                "payment/method/payment/process", request);

            if (!paymentResponse.IsSuccessStatusCode)
            {
                // Log evt. fejl
                return false;
            }

            // 3. (Valgfrit) Hent ordrebekræftelse (GET)
            var getResponse = await _httpClient.GetAsync(
                $"orderservice/method/order/{request.OrderId}");

            if (!getResponse.IsSuccessStatusCode)
            {
                // Ordre kunne ikke findes (burde ikke ske)
                return false;
            }

            return true;
        }

    }

    public class PaymentRequestDto
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public double Amount { get; set; }
    }
}


//request.CustomerId = _session.GetCustomerId();

//var response = await _httpClient.PostAsJsonAsync("payment/method/payment/process", request);
//return response.IsSuccessStatusCode;