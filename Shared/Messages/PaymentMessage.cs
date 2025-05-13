using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class PaymentMessage : WorkflowMessage
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public double Amount { get; set; }
    }

    public class PaymentResultMessage : PaymentMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
