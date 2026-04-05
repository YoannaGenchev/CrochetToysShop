using System.ComponentModel.DataAnnotations;

namespace CrochetToysShop.Web.ViewModels.Orders
{
    public class OrderCreateViewModel
    {
        public int ToyId { get; set; }

        public string? ToyName { get; set; }

        [Required, Display(Name = "Name"), StringLength(40, MinimumLength = 2)]
        public string CustomerName { get; set; } = null!;

        [Required, Display(Name = "Phone"), StringLength(10, MinimumLength = 6)]
        public string PhoneNumber { get; set; } = null!;

        [Required, Display(Name = "Address"), StringLength(200, MinimumLength = 5)]
        public string Address { get; set; } = null!;
    }
}
