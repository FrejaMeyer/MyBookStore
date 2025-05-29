namespace Inventory.Models
{
    public class InventoryItem
    {
        public string ProductId { get; set; }
        public bool Instock { get; set; }
        public int QuantityAvailable { get; set; }
    }
}
