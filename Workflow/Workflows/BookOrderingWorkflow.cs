using BookStoreWorkflow.Activities;
using Dapr.Workflow;
using Workflow.Activites;
using Workflow.Models;


namespace Workflow.Workflows;

public class BookOrderingWorkflow : Workflow<Order, Order>
{
    public override async Task<Order> RunAsync(WorkflowContext context, Order order)
    {
        try
        {
            // Step 1: Reserver lager
            order = await context.CallActivityAsync<Order>(
                nameof(InventoryReserveActivity), order);

            // Step 2: Behandl betaling
            order = await context.CallActivityAsync<Order>(
                nameof(PaymentActivity), order);

            if (order.Status != "paid")
            {
                // Step 3a: Hvis betaling fejler → fortryd reservation
                await context.CallActivityAsync<object>(
                    nameof(InventoryCancelActivity), order);

                order.Status = "payment_failed";
                return order;
            }

            // Step 3b: Hvis betaling lykkes → bekræft reservation
            await context.CallActivityAsync<object>(
                nameof(InventoryConfirmActivity), order);

            // Step 4: Opret ordren i systemet
            order = await context.CallActivityAsync<Order>(
                nameof(OrderActivity), order);

            // Step 5: Afslut ordre
            order.Status = "completed";
            return order;
        }
        catch (Exception ex)
        {
            order.Status = "failed";
            order.Error = ex.Message;
            return order;
        }
    }
}




//try
//{
//    // Step 1: Validate basket
//    await context.CallActivityAsync<object>(nameof(BasketActivity), order);
//    var basketResult = await context.WaitForExternalEventAsync<Order>("BasketValidated");
//    if (basketResult.Status != "validated")
//        throw new Exception($"Basket validation failed: {basketResult.Error}");

//    // Step 2: Create order
//    await context.CallActivityAsync<object>(nameof(OrderActivity), basketResult);
//    var orderResult = await context.WaitForExternalEventAsync<Order>("OrderCreated");
//    if (orderResult.Status != "confirmed")
//        throw new Exception($"Order creation failed: {orderResult.Error}");

//    // Step 3: Process payment
//    await context.CallActivityAsync<object>(nameof(PaymentActivity), orderResult);
//    var paymentResult = await context.WaitForExternalEventAsync<Order>("PaymentProcessed");
//    if (paymentResult.Status != "paid")
//        throw new Exception($"Payment failed: {paymentResult.Error}");

//    paymentResult.Status = "completed";
//    return paymentResult;
//}
//catch (Exception ex)
//{
//    order.Status = "error";
//    order.Error = ex.Message;
//    return order;
//}