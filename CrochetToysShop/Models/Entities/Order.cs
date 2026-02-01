using System.ComponentModel.DataAnnotations;

namespace CrochetToysShop.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string CustomerName { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "New";

        public int ToyId { get; set; }
        public Toy Toy { get; set; } = null!;
    }
}
