

namespace Workflow.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public Customer Customer { get; set; } 
        public List<CartItem> Items { get; set; }
        public string TotalPrice { get; set; }
        public string Status { get; set; }
        public string? Error { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Adress { get; set; }
    }
    
    public class CartItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }

    public class ValidationRequest
    {
        public string OrderId { get; set; }
        public bool Approved { get; set; }
    }

    public class ManageWorkflowRequest
    {
        public string OrderId { get; set; }
    }
}
