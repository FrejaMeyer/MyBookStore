using Frontend.Services;
using Shared.Dto;
using System.Net.Http.Json;




namespace Frontend.Services;

public class BasketClientService
{
    private HttpClient _http;
    private readonly CustomerSessionService _session;
    private readonly CartStateService _cartState; 


    public BasketClientService(CustomerSessionService session, CartStateService cartState)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:30002")
        };
        _session = session;
        _cartState = cartState;
    }

    public async Task UpdateCartAsync(CartItemDto item)
    {
        var customerId = _session.GetCustomerId();
        await _http.PutAsJsonAsync($"/basket/{customerId}/items", item);
    }


    public async Task<CartDto?> GetCartAsync()
    {
        var customerId = _session.GetCustomerId();
        return await _http.GetFromJsonAsync<CartDto>($"/basket/{customerId}");
    }

    public async Task AddToCartAsync(CartItemDto item)
    {
        var customerId = _session.GetCustomerId();
        await _http.PostAsJsonAsync($"/basket/{customerId}/items", item);
    }

    public async Task RemoveFromCartAsync(string productId)
    {
        var customerId = _session.GetCustomerId();
        await _http.DeleteAsync($"/basket/{customerId}/items/{productId}");
    }

    public async Task<string?> CheckoutAsync(CustomerDto customer)
    {
        try
        {
            // Sikrer at customerId bliver sat korrekt
            customer.CustomerId = _session.GetCustomerId();
            //customer.CustomerId = Guid.NewGuid().ToString();

            var response = await _http.PostAsJsonAsync("/basket/checkout", customer);
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


    public async Task ClearCartAsync()
    {
        var customerId = _session.GetCustomerId();
        await _http.DeleteAsync($"/basket/{customerId}");
        _cartState.NotifyCartChanged();
    }
}

