using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CrochetToysShop.Data;
using CrochetToysShop.Data.Models;
using CrochetToysShop.Services.Core;
using CrochetToysShop.Services.Core.Interfaces;
using static CrochetToysShop.Common.Constants.ApplicationConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException(ErrorMessages.MissingDefaultConnectionString);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IToyService, ToyService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Apply migrations and seed data only on first run
    if (!db.Database.GetPendingMigrations().Any())
    {
        // Database already migrated, proceed to seeding only
        SeedData(scope, db);
    }
    else
    {
        try
        {
            db.Database.Migrate();
            SeedData(scope, db);
        }
        catch (Exception ex)
        {
            // Log migration errors but don't crash app
            Console.WriteLine($"Migration error: {ex.Message}");
        }
    }
}

static void SeedData(IServiceScope scope, ApplicationDbContext db)
{
    var categoryNames = new[] { "Flowers", "Seasonal", "Bookmarks", "Animals", "Sweet Friends", "Accessories", "Easter", "Martenica" };

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    const string adminRole = Roles.Admin;
    const string userRole = "User";
    const string adminEmail = AdminSeed.Email;

    // Create Admin role
    if (!roleManager.Roles.Any(r => r.Name == adminRole))
    {
        roleManager.CreateAsync(new IdentityRole(adminRole)).GetAwaiter().GetResult();
    }

    // Create User role
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

        userManager.CreateAsync(adminUser, AdminSeed.Password).GetAwaiter().GetResult();
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

    // Seed courses
    var courseNames = new[] { "Beginner Amigurumi", "Advanced Amigurumi", "Crochet Blankets", "Baby Clothes Basics" };
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
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/error/500");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
