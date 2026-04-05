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

            var existingCourses = db.Courses.ToList();

            var seededCourses = new[]
            {
                new { Name = "Beginner Amigurumi", Description = "Learn the basics of creating adorable stuffed animals using crochet techniques. Perfect for beginners!", Price = 29.99m, DurationHours = 12, Difficulty = "Beginner", MaxStudents = 20, IsActive = true, ImageUrl = "/images/courses/beginer.jfif" },
                new { Name = "Advanced Amigurumi", Description = "Master complex amigurumi patterns and create sophisticated designs with shaping and details.", Price = 49.99m, DurationHours = 20, Difficulty = "Advanced", MaxStudents = 15, IsActive = true, ImageUrl = "/images/courses/advanced.jfif" },
                new { Name = "Crochet Blankets", Description = "Create beautiful cozy blankets with various patterns and techniques. Learn different stitches and patterns.", Price = 39.99m, DurationHours = 16, Difficulty = "Intermediate", MaxStudents = 18, IsActive = true, ImageUrl = "/images/courses/blanket.jfif" },
                new { Name = "Baby Clothes Basics", Description = "Learn to crochet adorable baby clothes including hats, booties, and sweaters.", Price = 34.99m, DurationHours = 14, Difficulty = "Intermediate", MaxStudents = 20, IsActive = true, ImageUrl = "/images/courses/baby.clothes.jfif" },
            };

            var coursesToAdd = new List<Course>();

            foreach (var seededCourse in seededCourses)
            {
                var existingCourse = existingCourses.FirstOrDefault(c => c.Name == seededCourse.Name);

                if (existingCourse == null)
                {
                    var courseToAdd = new Course
                    {
                        Name = seededCourse.Name,
                        Description = seededCourse.Description,
                        Price = seededCourse.Price,
                        DurationHours = seededCourse.DurationHours,
                        Difficulty = seededCourse.Difficulty,
                        MaxStudents = seededCourse.MaxStudents,
                        IsActive = seededCourse.IsActive,
                        ImageUrl = seededCourse.ImageUrl,
                    };

                    coursesToAdd.Add(courseToAdd);
                    existingCourses.Add(courseToAdd);
                    continue;
                }

                existingCourse.Description = seededCourse.Description;
                existingCourse.Price = seededCourse.Price;
                existingCourse.DurationHours = seededCourse.DurationHours;
                existingCourse.Difficulty = seededCourse.Difficulty;
                existingCourse.MaxStudents = seededCourse.MaxStudents;
                existingCourse.IsActive = seededCourse.IsActive;
                existingCourse.ImageUrl = seededCourse.ImageUrl;
            }

            if (coursesToAdd.Any())
            {
                db.Courses.AddRange(coursesToAdd);
            }

            db.SaveChanges();

            var categoriesByName = db.Categories
                .ToDictionary(c => c.Name, c => c.Id);

            var existingToys = db.Toys.ToList();
            var toysToAdd = new List<Toy>();

            var seededToys = new[]
            {
                new { Name = "Pink Bunny", Description = "Cute crochet bunny with soft ears.", Price = 15.00m, ImageUrl = "/images/toys/bunny.jpg", SizeCm = 24, Difficulty = "Hard", IsAvailable = true, CategoryName = "Animals" },
                new { Name = "Duck", Description = "Handmade crochet duck perfect for spring decor and gifts.", Price = 19.50m, ImageUrl = "/images/toys/duck.jpg", SizeCm = 30, Difficulty = "Hard", IsAvailable = true, CategoryName = "Easter" },
                new { Name = "Little Fox", Description = "Charming crochet fox toy with detailed tail.", Price = 10.90m, ImageUrl = "/images/toys/fox.jpg", SizeCm = 15, Difficulty = "Medium", IsAvailable = true, CategoryName = "Animals" },
                new { Name = "Mini Bear", Description = "Adorable bear for a sweet gift.", Price = 8.00m, ImageUrl = "/images/toys/bear1.jpg", SizeCm = 13, Difficulty = "Medium", IsAvailable = true, CategoryName = "Animals" },
                new { Name = "Harry Potter Bookmark", Description = "Lightweight crochet bookmark with a Harry Potter's hat top for readers.", Price = 8.50m, ImageUrl = "/images/toys/book1.jpg", SizeCm = 18, Difficulty = "Easy", IsAvailable = true, CategoryName = "Bookmarks" },
                new { Name = "A bouquet of tulips", Description = "Elegant crochet tulips bouquet with soft petals for home decor.", Price = 14.90m, ImageUrl = "/images/toys/flower.jpg", SizeCm = 21, Difficulty = "Medium", IsAvailable = true, CategoryName = "Flowers" },
                new { Name = "Rose Garden Charm", Description = "Hand-crocheted roses with layered petals, crafted as a sweet gift piece.", Price = 15.50m, ImageUrl = "/images/toys/rose1.jpg", SizeCm = 18, Difficulty = "Medium", IsAvailable = true, CategoryName = "Flowers" },
                new { Name = "Coffee Bookmark", Description = "Slim coffee-style crochet bookmark that keeps your place beautifully.", Price = 8.50m, ImageUrl = "/images/toys/book2.1.jpg", SizeCm = 19, Difficulty = "Easy", IsAvailable = true, CategoryName = "Bookmarks" },
                new { Name = "Rose Bookmark", Description = "Classic crochet rose bookmark with detailed edge stitching.", Price = 8.50m, ImageUrl = "/images/toys/book3.jpg", SizeCm = 20, Difficulty = "Easy", IsAvailable = false, CategoryName = "Bookmarks" },
                new { Name = "Pink Bunny With Hat", Description = "Soft bunny with long ears and hat, handmade for cozy play.", Price = 15.00m, ImageUrl = "/images/toys/bunny1.jpg", SizeCm = 25, Difficulty = "Medium", IsAvailable = true, CategoryName = "Easter" },
                new { Name = "Little Duckling", Description = "Cheerful crochet duckling with bright beak and rounded body.", Price = 18.90m, ImageUrl = "/images/toys/duck1.jpg", SizeCm = 35, Difficulty = "Medium", IsAvailable = false, CategoryName = "Animals" },
                new { Name = "Gray Bunny With Hat", Description = "Round plush bunny friend designed for hugs and nursery shelves.", Price = 15.00m, ImageUrl = "/images/toys/bunny2.jpg", SizeCm = 25, Difficulty = "Medium", IsAvailable = true, CategoryName = "Easter" },
                new { Name = "Mini Octo", Description = "A little sea friend perfect for holiday gifts.", Price = 4.00m, ImageUrl = "/images/toys/mini1.jpg", SizeCm = 8, Difficulty = "Easy", IsAvailable = true, CategoryName = "Sweet Friends" },
                new { Name = "Pocket Joy Friend", Description = "Small cheerful plush companion handmade for everyday smiles.", Price = 4.00m, ImageUrl = "/images/toys/mini2.jpg", SizeCm = 12, Difficulty = "Easy", IsAvailable = true, CategoryName = "Sweet Friends" },
                new { Name = "Pastel Egg", Description = "Decorative crochet egg in soft spring colors.", Price = 5.00m, ImageUrl = "/images/toys/egg1.jpg", SizeCm = 14, Difficulty = "Easy", IsAvailable = true, CategoryName = "Easter" },
                new { Name = "Nest Decor", Description = "Crochet nest centerpiece perfect for your terrace, repels wasps.", Price = 11.40m, ImageUrl = "/images/toys/nest.jpg", SizeCm = 25, Difficulty = "Medium", IsAvailable = true, CategoryName = "Accessories" },
                new { Name = "White Bunny", Description = "Big bunny designed to brighten Easter displays.", Price = 20.50m, ImageUrl = "/images/toys/easter.bunny.jpg", SizeCm = 20, Difficulty = "Medium", IsAvailable = false, CategoryName = "Easter" },
                new { Name = "Blue Bracelet", Description = "Blue-and-white crochet bracelet inspired by snow time.", Price = 3.00m, ImageUrl = "/images/toys/barclet.jpg", SizeCm = 10, Difficulty = "Easy", IsAvailable = true, CategoryName = "Accessories" },
                new { Name = "Martenica Twin Dolls", Description = "Handmade Martenica pair crafted as a symbol of spring and health.", Price = 5.00m, ImageUrl = "/images/toys/martenica1.jpg", SizeCm = 14, Difficulty = "Easy", IsAvailable = true, CategoryName = "Martenica" },
                new { Name = "Martenica Bracelet", Description = "Decorative spring bloom in traditional Martenica colors.", Price = 3.00m, ImageUrl = "/images/toys/martenica3.jpg", SizeCm = 11, Difficulty = "Easy", IsAvailable = true, CategoryName = "Martenica" },
                new { Name = "Cozy Hat Classic", Description = "Warm handcrafted crochet hat designed for chilly autumn mornings.", Price = 26.00m, ImageUrl = "/images/toys/hat1.jpg", SizeCm = 35, Difficulty = "Hard", IsAvailable = true, CategoryName = "Accessories" },
                new { Name = "Soft Winter Beanie", Description = "Chunky crochet beanie with textured stitch and snug winter fit.", Price = 28.50m, ImageUrl = "/images/toys/hat2.jpg", SizeCm = 35, Difficulty = "Hard", IsAvailable = false, CategoryName = "Accessories" },
                new { Name = "Everyday Belt Accent", Description = "Crochet belt accessory that adds a handmade touch to outfits.", Price = 18.40m, ImageUrl = "/images/toys/belt.jpg", SizeCm = 100, Difficulty = "Medium", IsAvailable = true, CategoryName = "Accessories" },
                new { Name = "Holiday Snowman", Description = "Festive crochet snowman decoration for winter shelves and gifts.", Price = 23.70m, ImageUrl = "/images/toys/snowman.jpg", SizeCm = 19, Difficulty = "Medium", IsAvailable = true, CategoryName = "Seasonal" },
                new { Name = "Christmas Elf Buddy", Description = "Playful elf ornament crocheted with holiday colors and charm.", Price = 22.90m, ImageUrl = "/images/toys/elf1.jpg", SizeCm = 20, Difficulty = "Medium", IsAvailable = true, CategoryName = "Seasonal" },
                new { Name = "Festive Tree Ornament", Description = "Crochet tree decor piece crafted for warm holiday atmospheres.", Price = 12.90m, ImageUrl = "/images/toys/tree.jpg", SizeCm = 15, Difficulty = "Easy", IsAvailable = true, CategoryName = "Seasonal" },
                new { Name = "Red shopping bag", Description = "Big crochet gift bag for shopping with flower.", Price = 30.00m, ImageUrl = "/images/toys/bag1.jpg", SizeCm = 50, Difficulty = "Hard", IsAvailable = true, CategoryName = "Accessories" },
                new { Name = "Heart Keepsake", Description = "Handmade crochet heart keepsake perfect for thoughtful gifts.", Price = 2.00m, ImageUrl = "/images/toys/heart.jpg", SizeCm = 5, Difficulty = "Easy", IsAvailable = true, CategoryName = "Accessories" },
            };

            var seededByImage = seededToys
                .Where(t => !string.IsNullOrWhiteSpace(t.ImageUrl))
                .ToDictionary(t => t.ImageUrl, t => t);

            var seededNames = new HashSet<string>(seededToys.Select(t => t.Name));

            var duplicateCandidates = existingToys
                .Where(t => t.CreatedByUserId == null)
                .Where(t => (!string.IsNullOrWhiteSpace(t.ImageUrl) && seededByImage.ContainsKey(t.ImageUrl!)) || seededNames.Contains(t.Name))
                .ToList();

            var toysToRemove = new List<Toy>();

            foreach (var imageGroup in duplicateCandidates
                .Where(t => !string.IsNullOrWhiteSpace(t.ImageUrl))
                .GroupBy(t => t.ImageUrl!)
                .Where(g => g.Count() > 1))
            {
                var seed = seededByImage[imageGroup.Key];

                var keeper = imageGroup.FirstOrDefault(t => t.Name == seed.Name)
                             ?? imageGroup.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id).First();

                foreach (var duplicate in imageGroup.Where(t => t.Id != keeper.Id))
                {
                    var ordersToReassign = db.Orders.Where(o => o.ToyId == duplicate.Id).ToList();
                    foreach (var order in ordersToReassign)
                    {
                        order.ToyId = keeper.Id;
                    }

                    var requestsToReassign = db.OrderRequests.Where(r => r.ToyId == duplicate.Id).ToList();
                    foreach (var request in requestsToReassign)
                    {
                        request.ToyId = keeper.Id;
                    }

                    toysToRemove.Add(duplicate);
                }
            }

            var imageDedupedCandidates = duplicateCandidates
                .Where(t => toysToRemove.All(r => r.Id != t.Id))
                .ToList();

            foreach (var nameGroup in imageDedupedCandidates
                .GroupBy(t => t.Name)
                .Where(g => g.Count() > 1))
            {
                var keeper = nameGroup.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id).First();

                foreach (var duplicate in nameGroup.Where(t => t.Id != keeper.Id))
                {
                    var ordersToReassign = db.Orders.Where(o => o.ToyId == duplicate.Id).ToList();
                    foreach (var order in ordersToReassign)
                    {
                        order.ToyId = keeper.Id;
                    }

                    var requestsToReassign = db.OrderRequests.Where(r => r.ToyId == duplicate.Id).ToList();
                    foreach (var request in requestsToReassign)
                    {
                        request.ToyId = keeper.Id;
                    }

                    toysToRemove.Add(duplicate);
                }
            }

            if (toysToRemove.Any())
            {
                db.Toys.RemoveRange(toysToRemove.DistinctBy(t => t.Id));
                db.SaveChanges();
                existingToys = db.Toys.ToList();
            }

            foreach (var seedToy in seededToys)
            {
                var existingToy = existingToys.FirstOrDefault(t =>
                    t.CreatedByUserId == null &&
                    t.ImageUrl == seedToy.ImageUrl)
                    ?? existingToys.FirstOrDefault(t =>
                        t.CreatedByUserId == null &&
                        t.Name == seedToy.Name);

                var categoryId = categoriesByName[seedToy.CategoryName];

                if (existingToy == null)
                {
                    var toyToAdd = new Toy
                    {
                        Name = seedToy.Name,
                        Description = seedToy.Description,
                        Price = seedToy.Price,
                        ImageUrl = seedToy.ImageUrl,
                        SizeCm = seedToy.SizeCm,
                        Difficulty = seedToy.Difficulty,
                        IsAvailable = seedToy.IsAvailable,
                        CategoryId = categoryId,
                    };

                    toysToAdd.Add(toyToAdd);
                    existingToys.Add(toyToAdd);
                    continue;
                }

                existingToy.Name = seedToy.Name;
                existingToy.Description = seedToy.Description;
                existingToy.Price = seedToy.Price;
                existingToy.ImageUrl = seedToy.ImageUrl;
                existingToy.SizeCm = seedToy.SizeCm;
                existingToy.Difficulty = seedToy.Difficulty;
                existingToy.IsAvailable = seedToy.IsAvailable;
                existingToy.CategoryId = categoryId;
            }

            if (toysToAdd.Any())
            {
                db.Toys.AddRange(toysToAdd);
            }

            db.SaveChanges();
        }
    }
}
