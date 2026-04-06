using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace CrochetToysShop.IntegrationTests;

public class PublicRoutesIntegrationTests : IClassFixture<IsolatedSqlServerWebApplicationFactory>
{
    private readonly IsolatedSqlServerWebApplicationFactory factory;

    public PublicRoutesIntegrationTests(IsolatedSqlServerWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GetHomeRoute_ReturnsSuccessStatusCode()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetToysRoute_ReturnsSuccessStatusCode()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/Toys");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetOrdersRoute_AsGuest_RedirectsToLogin()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Orders");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task GetToyCreateRoute_AsGuest_RedirectsToLogin()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Toys/Create");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task PostCourseEnroll_AsGuest_RedirectsToLogin()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsync("/Courses/Enroll/1", content: null);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task PostOrderMarkCompleted_AsGuest_RedirectsToLogin()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsync("/Orders/MarkCompleted/1", content: null);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task GetMyCourses_AsGuest_RedirectsToLogin()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Courses/MyCourses");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }
}
