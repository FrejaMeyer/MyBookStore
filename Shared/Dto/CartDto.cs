using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class CartDto
    {
        public string CustomerId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
    }
}
