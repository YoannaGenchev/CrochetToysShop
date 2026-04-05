namespace CrochetToysShop.Services.Models.Toys
{
    public class ToyListItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public string CategoryName { get; set; } = null!;

        public bool IsAvailable { get; set; }
    }
}
