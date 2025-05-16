namespace BookOrder.Models
{
    public class CartItems
    {
        public string ProductId { get; set; } 
        public string Name { get; set; } 
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
