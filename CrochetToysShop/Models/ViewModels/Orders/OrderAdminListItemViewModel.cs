namespace CrochetToysShop.Models.ViewModels.Orders
{
    public class OrderAdminListItemViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; } = null!;

        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;

        public int ToyId { get; set; }
        public string ToyName { get; set; } = null!;
    }
}
