using Frontend;
using Frontend.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.Toast;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient()
{
    BaseAddress = new Uri("http://localhost:30005")
});

builder.Services.AddScoped<BasketClientService>();
builder.Services.AddScoped<CartStateService>();
builder.Services.AddScoped<CustomerSessionService>();
builder.Services.AddScoped<PaymentService>();

//http://localhost:3500/v1.0/invoke/

builder.Services.AddBlazoredToast();

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
