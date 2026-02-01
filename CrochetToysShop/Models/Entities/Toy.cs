using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CrochetToysShop.Models.Entities
{
    public class Toy
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(2000, MinimumLength = 20)]
        public string Description { get; set; } = null!;

        [Range(typeof(decimal), "0.01", "100000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        [Range(1, 200)]
        public int SizeCm { get; set; }

        [Required]
        [StringLength(20)]
        public string Difficulty { get; set; } = "Easy";

        public bool IsAvailable { get; set; } = true;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public ICollection<OrderRequest> OrderRequests { get; set; } = new HashSet<OrderRequest>();
    }
}
