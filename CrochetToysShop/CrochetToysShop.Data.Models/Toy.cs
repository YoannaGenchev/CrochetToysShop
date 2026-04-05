using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CrochetToysShop.Common.Constants.EntityValidationConstants.Toy;

namespace CrochetToysShop.Data.Models
{
    public class Toy
    {
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength)]
        public string Description { get; set; } = null!;

        [Range(typeof(decimal), PriceMinValue, PriceMaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [RegularExpression(ImageUrlPattern, ErrorMessage = ImageUrlPatternErrorMessage)]
        public string? ImageUrl { get; set; }

        [Range(SizeMinCm, SizeMaxCm)]
        public int SizeCm { get; set; }

        [Required]
        [StringLength(DifficultyMaxLength)]
        public string Difficulty { get; set; } = DefaultDifficulty;

        public bool IsAvailable { get; set; } = true;

        public int CategoryId { get; set; }

        public string? CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Category Category { get; set; } = null!;

        public ICollection<OrderRequest> OrderRequests { get; set; } = new HashSet<OrderRequest>();
    }
}
