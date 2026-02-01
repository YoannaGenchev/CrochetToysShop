namespace CrochetToysShop.Models.ViewModels.Toys
{
    public class ToyDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int SizeCm { get; set; }
        public string Difficulty { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
