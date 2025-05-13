using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class OrderMessage : WorkflowMessage
    {
        public string OrderId { get; set; }
        public CustomerDto Customer { get; set; }
        public List<CartItemDto> Items { get; set; }
        public string TotalPrice { get; set; }
    }

    public class OrderResultMessage : OrderMessage
    {
        public string Status { get; set; } 
        public string? Error { get; set; }
    }
}
