using CrochetToysShop.Data.Models;
using CrochetToysShop.Data.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static CrochetToysShop.Common.Constants.ApplicationConstants;

namespace CrochetToysShop.Data.Seeding
{
    public static class DbSeeder
    {
        public static void Seed(IServiceScope scope, ApplicationDbContext db)
        {
            var categoryNames = new[] { "Flowers", "Seasonal", "Bookmarks", "Animals", "Sweet Friends", "Accessories", "Easter", "Martenica" };

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var adminSettings = configuration
                .GetSection(AdminSeedSettings.SectionName)
                .Get<AdminSeedSettings>();

            if (adminSettings == null)
            {
                throw new InvalidOperationException("AdminSeed settings are missing.");
            }

            const string adminRole = Roles.Admin;
            const string userRole = "User";
            var adminEmail = adminSettings.Email;
            var adminPassword = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException("Admin seed password is missing. Set ADMIN_SEED_PASSWORD.");
            }

            if (!roleManager.Roles.Any(r => r.Name == adminRole))
            {
                roleManager.CreateAsync(new IdentityRole(adminRole)).GetAwaiter().GetResult();
            }

            if (!roleManager.Roles.Any(r => r.Name == userRole))
            {
                roleManager.CreateAsync(new IdentityRole(userRole)).GetAwaiter().GetResult();
            }

            var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };

                userManager.CreateAsync(adminUser, adminPassword).GetAwaiter().GetResult();
            }
            else
            {
                var hasPassword = userManager.HasPasswordAsync(adminUser).GetAwaiter().GetResult();
                IdentityResult passwordResult;

                if (hasPassword)
                {
                    var resetToken = userManager.GeneratePasswordResetTokenAsync(adminUser).GetAwaiter().GetResult();
                    passwordResult = userManager.ResetPasswordAsync(adminUser, resetToken, adminPassword).GetAwaiter().GetResult();
                }
                else
                {
                    passwordResult = userManager.AddPasswordAsync(adminUser, adminPassword).GetAwaiter().GetResult();
                }

                if (!passwordResult.Succeeded)
                {
                    var errors = string.Join("; ", passwordResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to synchronize admin password: {errors}");
                }
            }

            if (!userManager.IsInRoleAsync(adminUser, adminRole).GetAwaiter().GetResult())
            {
                userManager.AddToRoleAsync(adminUser, adminRole).GetAwaiter().GetResult();
            }

            if (!userManager.IsInRoleAsync(adminUser, userRole).GetAwaiter().GetResult())
            {
                userManager.AddToRoleAsync(adminUser, userRole).GetAwaiter().GetResult();
            }

            var existing = db.Categories.Select(c => c.Name).ToList();

            var toAdd = categoryNames
                .Where(name => !existing.Contains(name))
                .Select(name => new Category { Name = name })
                .ToList();

            if (toAdd.Any())
            {
                db.Categories.AddRange(toAdd);
                db.SaveChanges();
            }

            var existingCourses = db.Courses.Select(c => c.Name).ToList();

            var coursesToAdd = new List<Course>();

            if (!existingCourses.Contains("Beginner Amigurumi"))
            {
                coursesToAdd.Add(new Course
                {
                    Name = "Beginner Amigurumi",
                    Description = "Learn the basics of creating adorable stuffed animals using crochet techniques. Perfect for beginners!",
                    Price = 29.99m,
                    DurationHours = 12,
                    Difficulty = "Beginner",
                    MaxStudents = 20,
                    IsActive = true
                });
            }

            if (!existingCourses.Contains("Advanced Amigurumi"))
            {
                coursesToAdd.Add(new Course
                {
                    Name = "Advanced Amigurumi",
                    Description = "Master complex amigurumi patterns and create sophisticated designs with shaping and details.",
                    Price = 49.99m,
                    DurationHours = 20,
                    Difficulty = "Advanced",
                    MaxStudents = 15,
                    IsActive = true
                });
            }

            if (!existingCourses.Contains("Crochet Blankets"))
            {
                coursesToAdd.Add(new Course
                {
                    Name = "Crochet Blankets",
                    Description = "Create beautiful cozy blankets with various patterns and techniques. Learn different stitches and patterns.",
                    Price = 39.99m,
                    DurationHours = 16,
                    Difficulty = "Intermediate",
                    MaxStudents = 18,
                    IsActive = true
                });
            }

            if (!existingCourses.Contains("Baby Clothes Basics"))
            {
                coursesToAdd.Add(new Course
                {
                    Name = "Baby Clothes Basics",
                    Description = "Learn to crochet adorable baby clothes including hats, booties, and sweaters.",
                    Price = 34.99m,
                    DurationHours = 14,
                    Difficulty = "Intermediate",
                    MaxStudents = 20,
                    IsActive = true
                });
            }

            if (coursesToAdd.Any())
            {
                db.Courses.AddRange(coursesToAdd);
                db.SaveChanges();
            }

            var categoriesByName = db.Categories
                .ToDictionary(c => c.Name, c => c.Id);

            var existingToyNames = db.Toys
                .Select(t => t.Name)
                .ToList();

            var toysToAdd = new List<Toy>();

            if (!existingToyNames.Contains("Spring Bunny"))
            {
                toysToAdd.Add(new Toy
                {
                    Name = "Spring Bunny",
                    Description = "Cute crochet bunny with soft ears and a tiny pastel bow.",
                    Price = 25.00m,
                    ImageUrl = "/images/toys/bunny.jpg",
                    SizeCm = 24,
                    Difficulty = "Easy",
                    IsAvailable = true,
                    CategoryId = categoriesByName["Animals"]
                });
            }

            if (!existingToyNames.Contains("Sunny Chick"))
            {
                toysToAdd.Add(new Toy
                {
                    Name = "Sunny Chick",
                    Description = "Handmade crochet chick perfect for spring decor and gifts.",
                    Price = 19.50m,
                    ImageUrl = "/images/toys/duck.jpg",
                    SizeCm = 16,
                    Difficulty = "Easy",
                    IsAvailable = true,
                    CategoryId = categoriesByName["Easter"]
                });
            }

            if (!existingToyNames.Contains("Little Fox"))
            {
                toysToAdd.Add(new Toy
                {
                    Name = "Little Fox",
                    Description = "Charming crochet fox toy with detailed tail and cozy scarf.",
                    Price = 29.90m,
                    ImageUrl = "/images/toys/fox.jpg",
                    SizeCm = 22,
                    Difficulty = "Medium",
                    IsAvailable = true,
                    CategoryId = categoriesByName["Animals"]
                });
            }

            if (!existingToyNames.Contains("Rose Bear"))
            {
                toysToAdd.Add(new Toy
                {
                    Name = "Rose Bear",
                    Description = "Adorable bear with crochet rose details for a sweet gift.",
                    Price = 32.00m,
                    ImageUrl = "/images/toys/bear1.jpg",
                    SizeCm = 26,
                    Difficulty = "Medium",
                    IsAvailable = true,
                    CategoryId = categoriesByName["Sweet Friends"]
                });
            }

            if (!existingToyNames.Contains("Daisy Bookmark"))
            {
                toysToAdd.Add(new Toy
                {
                    Name = "Daisy Bookmark",
                    Description = "Lightweight crochet bookmark with a daisy top for readers.",
                    Price = 8.90m,
                    ImageUrl = "/images/toys/book1.jpg",
                    SizeCm = 18,
                    Difficulty = "Easy",
                    IsAvailable = true,
                    CategoryId = categoriesByName["Bookmarks"]
                });
            }

            if (toysToAdd.Any())
            {
                db.Toys.AddRange(toysToAdd);
                db.SaveChanges();
            }
        }
    }
}
