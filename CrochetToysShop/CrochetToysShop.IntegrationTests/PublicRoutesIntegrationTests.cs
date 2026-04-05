using CrochetToysShop.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CrochetToysShop.IntegrationTests;

public class PublicRoutesIntegrationTests : IClassFixture<WebApplicationFactory<HomeController>>
{
    private readonly WebApplicationFactory<HomeController> factory;

    public PublicRoutesIntegrationTests(WebApplicationFactory<HomeController> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetHomeRoute_ReturnsSuccessStatusCode()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetToysRoute_ReturnsSuccessStatusCode()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Toys");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
