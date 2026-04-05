using CrochetToysShop.Data;
using CrochetToysShop.Data.Seeding;
using Microsoft.EntityFrameworkCore;

namespace CrochetToysShop.Web.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication ApplyMigrationsAndSeed(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            db.Database.Migrate();
            DbSeeder.Seed(scope, db);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Startup database migration/seeding failed.");
        }

        return app;
    }
}
