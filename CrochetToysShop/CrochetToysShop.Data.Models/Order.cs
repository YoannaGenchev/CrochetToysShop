using System.ComponentModel.DataAnnotations;
using CrochetToysShop.Common;
using static CrochetToysShop.Common.Constants.EntityValidationConstants.Order;

namespace CrochetToysShop.Data.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CustomerNameMaxLength)]
        public string CustomerName { get; set; } = null!;

        [Required]
        [StringLength(PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(AddressMaxLength)]
        public string Address { get; set; } = null!;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(StatusMaxLength)]
        public string Status { get; set; } = OrderStatus.New;

        public int ToyId { get; set; }

        public string? UserId { get; set; }

        public Toy Toy { get; set; } = null!;
    }
}
