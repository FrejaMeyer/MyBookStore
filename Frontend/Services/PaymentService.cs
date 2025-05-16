using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SimulatePaymentAsync(PaymentRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("payment/method/payment", request);
            return response.IsSuccessStatusCode;
        }
    }

    public class PaymentRequestDto
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public double Amount { get; set; }
    }
}
