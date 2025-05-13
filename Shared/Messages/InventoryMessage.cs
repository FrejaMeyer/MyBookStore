using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class InventoryMessage : WorkflowMessage
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class InventoryResultMessage : InventoryMessage
    {
        public string Status { get; set; }
        public string? Error { get; set; }
    }
}
