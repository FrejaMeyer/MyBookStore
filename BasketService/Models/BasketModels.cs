namespace Basket.Models
{
    public class Cart
    {
        public Customer Customer { get; set; }
        public List<CartItems> Items { get; set; }
    }

    public class CartItems
    {
        public string ProductId { get; set; }
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
}
