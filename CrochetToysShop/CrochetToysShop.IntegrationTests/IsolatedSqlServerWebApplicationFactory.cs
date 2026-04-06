using CrochetToysShop.Web.Controllers;
using CrochetToysShop.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CrochetToysShop.IntegrationTests;

public sealed class IsolatedSqlServerWebApplicationFactory : WebApplicationFactory<HomeController>
{
    private readonly string databaseName = $"CrochetToysShop_IntegrationTests_{Guid.NewGuid():N}";
    private string ConnectionString =>
        $"Server=(localdb)\\MSSQLLocalDB;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ConnectionString,
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(ConnectionString));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        try
        {
            using var masterConnection = new Microsoft.Data.SqlClient.SqlConnection(
                "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;Encrypt=False;");
            masterConnection.Open();

            using var dropCommand = masterConnection.CreateCommand();
            dropCommand.CommandText = $@"
                IF DB_ID('{databaseName}') IS NOT NULL
                BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
                END";
            dropCommand.ExecuteNonQuery();
        }
        catch
        {
        }
    }
}
