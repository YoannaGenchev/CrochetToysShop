using System.ComponentModel.DataAnnotations;
using static CrochetToysShop.Common.Constants.EntityValidationConstants.Category;

namespace CrochetToysShop.Data.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<Toy> Toys { get; set; } = new HashSet<Toy>();
    }
}
