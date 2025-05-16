using Frontend.Services;
using Shared.Dto;
using System.Net.Http;
using System.Net.Http.Json;


namespace Frontend.Services;

public class BasketClientService
{
    private readonly HttpClient _http;
    private readonly CustomerSessionService _session;

    public BasketClientService(HttpClient http, CustomerSessionService session)
    {
        _http = http;
        _session = session;
    }

    public async Task UpdateCartAsync(CartItemDto item)
    {
        var customerId = _session.GetCustomerId();
        await _http.PutAsJsonAsync($"basket/method/basket/{customerId}/items", item);
    }


    public async Task<CartDto?> GetCartAsync()
    {
        var customerId = _session.GetCustomerId();
        return await _http.GetFromJsonAsync<CartDto>($"basketservice/method/basket/{customerId}");
    }

    public async Task AddToCartAsync(CartItemDto item)
    {
        var customerId = _session.GetCustomerId();
        await _http.PostAsJsonAsync($"basketservice/method/basket/{customerId}/items", item);
    }

    public async Task RemoveFromCartAsync(string productId)
    {
        var customerId = _session.GetCustomerId();
        await _http.DeleteAsync($"basketservice/method/basket/{customerId}/items/{productId}");
    }

    public async Task<string?> CheckoutAsync(CustomerDto customer)
    {
        try
        {
            // Sikrer at customerId bliver sat korrekt
            customer.CustomerId = _session.GetCustomerId();

            var response = await _http.PostAsJsonAsync("basketservice/method/basket/checkout", customer);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"[Checkout] Status: {response.StatusCode}");
            Console.WriteLine($"[Checkout] Body: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("[Checkout] Fejl under kald – status ikke OK");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<CheckoutResponse>();
            return result?.OrderId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Checkout] Exception: {ex.Message}");
            return null;
        }
    }

    private class CheckoutResponse
    {
        public string OrderId { get; set; }
    }
}

