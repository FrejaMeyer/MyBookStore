using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class InventoryItemDto
    {
        public string ProductId { get; set; }
        public bool Instock { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantityReserved { get; set; }
    }
}
