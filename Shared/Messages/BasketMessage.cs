using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Dto;

namespace Shared.Messages
{
    public class BasketMessage : WorkflowMessage
    {
        public string OrderId { get; set; }
        public double TotalPrice { get; set; }
        public CustomerDto Customer { get; set; }
        public List<CartItemDto> Items { get; set; }

    }

    public class CartResultMessage : BasketMessage
    {
        public string Status { get; set; } 
        public string? Error { get; set; }
    }
}
