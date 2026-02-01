using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CrochetToysShop.Models.Entities;


namespace CrochetToysShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Toy> Toys { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<OrderRequest> OrderRequests { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;


    }
}
