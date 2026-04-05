using System.ComponentModel.DataAnnotations;

namespace CrochetToysShop.Web.ViewModels.Toys
{
    public class ToyFormViewModel
    {
        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(2000, MinimumLength = 20)]
        public string Description { get; set; } = null!;

        [Range(typeof(decimal), "0.01", "100000")]
        public decimal Price { get; set; }

        [Display(Name = "Image (path)")]
        [RegularExpression(@"^/images/toys/.*\.(jpg|jpeg|png|webp)$", ErrorMessage = "Please enter a path like /images/toys/name.jpg")]
        public string? ImageUrl { get; set; }

        [Range(1, 200)]
        public int SizeCm { get; set; }

        [Required]
        public string Difficulty { get; set; } = "Easy";

        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Category")]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryDropdownViewModel> Categories { get; set; } = new List<CategoryDropdownViewModel>();
    }

    public class CategoryDropdownViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }
}
