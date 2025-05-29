using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public class ConfirmReservationMessage
    {
        public string OrderId { get; set; }
        public List<ReservedItems> Items { get; set; } 
    }

    public class ReserveStockMessage
    {
        public string OrderId { get; set; }
        public List<ReservedItems> Items { get; set; }
    }

    public class CancelReservationMessage
    {
        public string OrderId { get; set; }
        public List<ReservedItems> Items { get; set; }
    }

    public class ReservedItems
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
