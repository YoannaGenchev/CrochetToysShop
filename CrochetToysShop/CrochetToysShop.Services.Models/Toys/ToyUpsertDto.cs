namespace CrochetToysShop.Services.Models.Toys
{
    public class ToyUpsertDto
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int SizeCm { get; set; }

        public string Difficulty { get; set; } = "Easy";

        public bool IsAvailable { get; set; } = true;

        public int CategoryId { get; set; }

        public IEnumerable<CategoryOptionDto> Categories { get; set; } = new List<CategoryOptionDto>();
    }
}
