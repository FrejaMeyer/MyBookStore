using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class OrderDto
    {
        public string OrderId { get; set; }
        public CustomerDto Customer { get; set; } 
        public List<CartItemDto> Items { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }
        public string? Error { get; set; }
    }
}
