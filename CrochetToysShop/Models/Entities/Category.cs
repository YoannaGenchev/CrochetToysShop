using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace CrochetToysShop.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Name { get; set; } = null!;

        public ICollection<Toy> Toys { get; set; } = new HashSet<Toy>();
    }
}
