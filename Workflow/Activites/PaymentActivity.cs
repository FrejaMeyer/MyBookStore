using Dapr.Client;
using Dapr.Workflow;
using Workflow.Helpers;
using Shared.Messages;
using Workflow.Models;

namespace BookStoreWorkflow.Activities;

public class PaymentActivity : WorkflowActivity<Order, Order>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<PaymentActivity> _logger;

    public PaymentActivity(DaprClient daprClient, ILogger<PaymentActivity> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public override async Task<Order> RunAsync(WorkflowActivityContext context, Order order)
    {
        try
        {
            _logger.LogInformation("Processing payment for order {OrderId}", order.OrderId);


            var response = await _daprClient.InvokeMethodAsync<Order, PaymentResultMessage>(
                HttpMethod.Post,
                "payment",          // app-id på din payment-service
                "payment/process",          // endpoint
                order);

            if (response.Status == "paid")
            {
                _logger.LogInformation("Payment successful for order {OrderId}", order.OrderId);
                order.Status = "paid";
                return order;
            }
            else
            {
                _logger.LogWarning("Payment failed for order {OrderId}: {Reason}", order.OrderId, response.Error);
                order.Status = "payment_failed";
                order.Error = response.Error ?? "Unknown payment error";
                return order;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order {OrderId}", order.OrderId);
            order.Status = "payment_error";
            order.Error = ex.Message;
            return order;
        }
    }

}

    //public override async Task<object?> RunAsync(WorkflowActivityContext context, Order order)
    //{
    //    _logger.LogInformation("Processing payment for order {OrderId}", order.OrderId);
    //    var message = MessageHelper.FillMessage<PaymentMessage>(context, order);
    //    await _daprClient.PublishEventAsync("bookpubsub", "process-payment", message);
    //    return null;
    //}
