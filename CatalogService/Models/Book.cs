namespace Catalog.Models
{
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Author { get; set; }
        public double Price { get; set; }
        public int? StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }

    }
}
