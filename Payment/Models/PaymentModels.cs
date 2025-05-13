namespace Payment.Models
{
    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public double Amount { get; set; }
    }

    public class PaymentResult
    {
        public string OrderId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
