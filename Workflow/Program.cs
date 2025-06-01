using BookStoreWorkflow.Activities;
using Dapr.Workflow;
using Workflow.Activites;
using Workflow.Workflows;

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

builder.Services.AddControllers().AddDapr();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDaprWorkflow(options =>
{
    // Register workflows
    options.RegisterWorkflow<BookOrderingWorkflow>();

    // Register activities
    options.RegisterActivity<BasketActivity>();
    options.RegisterActivity<OrderActivity>();
    options.RegisterActivity<PaymentActivity>();
    options.RegisterActivity<InventoryReserveActivity>();
    options.RegisterActivity<InventoryCancelActivity>();
    options.RegisterActivity<InventoryConfirmActivity>();
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");


app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapControllers();
app.Run();