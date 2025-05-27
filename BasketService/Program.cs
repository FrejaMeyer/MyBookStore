using Basket.Services;
using Dapr.AspNetCore;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5227") 
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddDapr();
builder.Services.AddSingleton<IBasketService, BasketService>();



var app = builder.Build();


app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.MapControllers();


app.Run();