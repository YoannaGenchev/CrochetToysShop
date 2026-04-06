using CrochetToysShop.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<Course> Courses { get; set; } = null!;

        public DbSet<Enrollment> Enrollments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.CourseId, e.UserId })
                .IsUnique();
        }
    }
}
