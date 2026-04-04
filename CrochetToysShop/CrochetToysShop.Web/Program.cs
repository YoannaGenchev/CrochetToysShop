using CrochetToysShop.Data;
using CrochetToysShop.Data.Seeding;
using CrochetToysShop.Services.Core;
using CrochetToysShop.Services.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static CrochetToysShop.Common.Constants.ApplicationConstants;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(ErrorMessages.MissingDefaultConnectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var identitySettings = builder.Configuration
    .GetSection(IdentitySettings.SectionName)
    .Get<IdentitySettings>() ?? new IdentitySettings();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = identitySettings.SignIn.RequireConfirmedAccount;
    options.Password.RequireDigit = identitySettings.Password.RequireDigit;
    options.Password.RequireLowercase = identitySettings.Password.RequireLowercase;
    options.Password.RequireUppercase = identitySettings.Password.RequireUppercase;
    options.Password.RequireNonAlphanumeric = identitySettings.Password.RequireNonAlphanumeric;
    options.Password.RequiredLength = identitySettings.Password.RequiredLength;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IToyService, ToyService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICourseService, CourseService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.Database.GetPendingMigrations().Any())
    {
        DbSeeder.Seed(scope, db);
    }
    else
    {
        try
        {
            db.Database.Migrate();
            DbSeeder.Seed(scope, db);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration error: {ex.Message}");
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/error/500");
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
