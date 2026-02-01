using System.ComponentModel.DataAnnotations;

namespace CrochetToysShop.Models.Entities
{
    public class OrderRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(60)]
        public string CustomerName { get; set; } = null!;


        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 5)]
        public string Message { get; set; } = null!;

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "New";

        // запитване?? 
        public int? ToyId { get; set; }
        public Toy? Toy { get; set; }
    }
}
