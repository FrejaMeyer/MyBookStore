using Dapr.Workflow;
using Shared.Messages;
using Workflow.Models;
using Shared.Dto;
using System.Linq;

namespace Workflow.Helpers
{
    public class MessageHelper
    {
        // Mappper en order-like model to message 
        public static T FillMessage<T>(WorkflowActivityContext context, Order order)
            where T : WorkflowMessage, new()
        {
            var result = new T();
            var msgType = typeof(T);

            msgType.GetProperty("WorkflowId")?.SetValue(result, order.OrderId);
            msgType.GetProperty("OrderId")?.SetValue(result, order.OrderId);
            msgType.GetProperty("TotalPrice")?.SetValue(result, order.TotalPrice);
            if (msgType.GetProperty("Customer") != null && order.Customer != null)
            {
                var dto = new CustomerDto
                {
                    Name = order.Customer.Name,
                    Email = order.Customer.Email,
                    Address = order.Customer.Adress
                };
                msgType.GetProperty("Customer")?.SetValue(result, dto);
            }

            if (msgType.GetProperty("Items") != null && order.Items != null)
            {
                var items = order.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList();

            msgType.GetProperty("Items")?.SetValue(result, items);

            }

            return result;
        }

        public static Order FillOrder<T>(T message) where T : WorkflowMessage
        {
            var msgType = typeof(T);

            var orderId = (string)msgType.GetProperty("OrderId")?.GetValue(message);
            var customerDto = (CustomerDto)msgType.GetProperty("Customer")?.GetValue(message);

            var customer = new Customer
            {
                Name = customerDto?.Name ?? "",
                Email = customerDto?.Email ?? "",
                Adress = customerDto?.Address ?? ""
            };

            var itemsDto = (List<CartItemDto>)msgType.GetProperty("Items")?.GetValue(message);
            var items = itemsDto?.Select(i => new CartItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();
            
            var status = (string)msgType.GetProperty("Status")?.GetValue(message) ?? "Unknow";
            var error = (string)msgType.GetProperty("Error")?.GetValue(message);

            return new Order
            {
                OrderId = orderId,
                Customer = customer,
                Items = items ?? new List<CartItem>(),
                TotalPrice = (string)msgType.GetProperty("TotalPrice")?.GetValue(message) ?? "0",
                Status = status,
                Error = error
            };
        }
    }
}

