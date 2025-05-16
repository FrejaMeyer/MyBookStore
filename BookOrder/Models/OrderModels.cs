using OpenTelemetry.Metrics;

namespace BookOrder.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; }
        public double TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }

    public class OrderItem
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }

    public class Customer
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }

    public enum OrderStatus
    {
        Submitted,
        Validated,
        Pending,
        Shipped,
        Completed

    }
}

