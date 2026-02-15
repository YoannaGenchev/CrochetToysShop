using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CrochetToysShop.Data;
using CrochetToysShop.Models.Entities;
using CrochetToysShop.Services.Interfaces;
using CrochetToysShop.Services.Implementations;



namespace CrochetToysShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddScoped<IOrderService, OrderService>();

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

            builder.Services.AddScoped<IToyService, ToyService>();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();

                var categoryNames = new[]
                {
                    "Цветя",
                    "Сезонни",
                    "Книгоразделители",
                    "Животни",
                    "Сладки приятели",
                    "Аксесоари"
                };

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                const string adminRole = "Admin";
                const string adminEmail = "yoanna@admin.com";

                if (!roleManager.Roles.Any(r => r.Name == adminRole))
                {
                    roleManager.CreateAsync(new IdentityRole(adminRole)).GetAwaiter().GetResult();
                }

                var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();

                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    userManager.CreateAsync(adminUser, "admin1").GetAwaiter().GetResult();
                }

                if (!userManager.IsInRoleAsync(adminUser, adminRole).GetAwaiter().GetResult())
                {
                    userManager.AddToRoleAsync(adminUser, adminRole).GetAwaiter().GetResult();
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
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

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
        }
    }
}
