using System.ComponentModel.DataAnnotations;
using CrochetToysShop.Common;
using static CrochetToysShop.Common.Constants.EntityValidationConstants.OrderRequest;

namespace CrochetToysShop.Data.Models
{
    public class OrderRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CustomerNameMaxLength)]
        public string CustomerName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

        [Required]
        [StringLength(MessageMaxLength, MinimumLength = MessageMinLength)]
        public string Message { get; set; } = null!;

        [Range(QuantityMin, QuantityMax)]
        public int Quantity { get; set; } = 1;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(StatusMaxLength)]
        public string Status { get; set; } = OrderStatus.New;

        public int? ToyId { get; set; }

        public Toy? Toy { get; set; }
    }
}
