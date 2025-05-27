using BookStoreWorkflow.Activities;
using Dapr.Workflow;
using Workflow.Activites;
using Workflow.Workflows;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapControllers();
app.Run();