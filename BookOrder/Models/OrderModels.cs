namespace BookOrder.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; }
        public string TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }

    public class OrderItem
    {
        public string ProductId { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        awaitpayment,
        Shipped,
        Delivered,
        Completed,
        Cancelled

    }
}

