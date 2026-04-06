using System.Net;
using System.Text.RegularExpressions;
using CrochetToysShop.Common.Constants;
using CrochetToysShop.Data;
using CrochetToysShop.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CrochetToysShop.IntegrationTests;

public class RegistrationRoleEnrollmentIntegrationTests : IClassFixture<IsolatedSqlServerWebApplicationFactory>
{
    private readonly IsolatedSqlServerWebApplicationFactory factory;

    public RegistrationRoleEnrollmentIntegrationTests(IsolatedSqlServerWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Register_AssignsUserRole_AndEnablesCourseEnrollment()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

        var email = $"registration-role-{Guid.NewGuid():N}@example.com";
        const string password = "123456";
        var courseId = EnsureTestCourse();

        var registerPageResponse = await client.GetAsync("/Identity/Account/Register");
        registerPageResponse.EnsureSuccessStatusCode();
        var registerPageHtml = await registerPageResponse.Content.ReadAsStringAsync();
        var registerToken = ExtractAntiforgeryToken(registerPageHtml);

        var registerForm = new Dictionary<string, string>
        {
            ["Input.Email"] = email,
            ["Input.Password"] = password,
            ["Input.ConfirmPassword"] = password,
            ["__RequestVerificationToken"] = registerToken,
        };

        var registerResponse = await client.PostAsync(
            "/Identity/Account/Register",
            new FormUrlEncodedContent(registerForm));

        Assert.Equal(HttpStatusCode.Redirect, registerResponse.StatusCode);

        string userId;
        using (var scope = factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);

            Assert.NotNull(user);
            Assert.True(await userManager.IsInRoleAsync(user!, ApplicationConstants.Roles.User));

            userId = user!.Id;
        }

        var detailsResponse = await client.GetAsync($"/Courses/Details/{courseId}");
        detailsResponse.EnsureSuccessStatusCode();
        var detailsHtml = await detailsResponse.Content.ReadAsStringAsync();
        var enrollToken = ExtractAntiforgeryToken(detailsHtml);

        var enrollResponse = await client.PostAsync(
            $"/Courses/Enroll/{courseId}",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = enrollToken,
            }));

        Assert.Equal(HttpStatusCode.Redirect, enrollResponse.StatusCode);

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var enrollmentExists = dbContext.Enrollments.Any(e => e.CourseId == courseId && e.UserId == userId);
            Assert.True(enrollmentExists);
        }
    }

    [Fact]
    public async Task AdminUser_CannotEnroll_EvenWithDirectPostRequest()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });

        var courseId = EnsureTestCourse();
        var (adminEmail, adminPassword, adminUserId) = await EnsureAdminUserWithUserRoleAsync();

        var loginPageResponse = await client.GetAsync("/Identity/Account/Login");
        loginPageResponse.EnsureSuccessStatusCode();
        var loginPageHtml = await loginPageResponse.Content.ReadAsStringAsync();
        var loginToken = ExtractAntiforgeryToken(loginPageHtml);

        var loginResponse = await client.PostAsync(
            "/Identity/Account/Login",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Input.Email"] = adminEmail,
                ["Input.Password"] = adminPassword,
                ["Input.RememberMe"] = "false",
                ["__RequestVerificationToken"] = loginToken,
            }));

        Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);

        var detailsResponse = await client.GetAsync($"/Courses/Details/{courseId}");
        detailsResponse.EnsureSuccessStatusCode();
        var detailsHtml = await detailsResponse.Content.ReadAsStringAsync();
        var enrollToken = ExtractAntiforgeryToken(detailsHtml);

        var enrollResponse = await client.PostAsync(
            $"/Courses/Enroll/{courseId}",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = enrollToken,
            }));

        Assert.Equal(HttpStatusCode.Redirect, enrollResponse.StatusCode);
        Assert.Equal($"/Courses/Details/{courseId}", enrollResponse.Headers.Location?.ToString());

        var redirectResponse = await client.GetAsync(enrollResponse.Headers.Location);
        redirectResponse.EnsureSuccessStatusCode();
        var redirectHtml = await redirectResponse.Content.ReadAsStringAsync();
        Assert.Contains("Admins cannot enroll in courses.", redirectHtml);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var enrollmentExists = dbContext.Enrollments.Any(e => e.CourseId == courseId && e.UserId == adminUserId);
        Assert.False(enrollmentExists);
    }

    private int EnsureTestCourse()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        const string courseName = "Integration Test Enrollment Course";
        var existing = dbContext.Courses.FirstOrDefault(c => c.Name == courseName);
        if (existing != null)
        {
            existing.IsActive = true;
            existing.MaxStudents = Math.Max(existing.MaxStudents, 1000);
            dbContext.SaveChanges();
            return existing.Id;
        }

        var course = new Course
        {
            Name = courseName,
            Description = "Course used by integration test for role-based enrollment.",
            Price = 19.99m,
            DurationHours = 6,
            Difficulty = "Beginner",
            MaxStudents = 1000,
            IsActive = true,
            ImageUrl = "/images/toys/no-image.svg",
        };

        dbContext.Courses.Add(course);
        dbContext.SaveChanges();

        return course.Id;
    }

    private async Task<(string Email, string Password, string UserId)> EnsureAdminUserWithUserRoleAsync()
    {
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var email = $"admin-enroll-block-{Guid.NewGuid():N}@example.com";
        const string password = "123456";

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
        };

        var createResult = await userManager.CreateAsync(user, password);
        Assert.True(createResult.Succeeded, string.Join("; ", createResult.Errors.Select(e => e.Description)));

        if (!await roleManager.RoleExistsAsync(ApplicationConstants.Roles.Admin))
        {
            var adminRoleResult = await roleManager.CreateAsync(new IdentityRole(ApplicationConstants.Roles.Admin));
            Assert.True(adminRoleResult.Succeeded, string.Join("; ", adminRoleResult.Errors.Select(e => e.Description)));
        }

        if (!await roleManager.RoleExistsAsync(ApplicationConstants.Roles.User))
        {
            var userRoleResult = await roleManager.CreateAsync(new IdentityRole(ApplicationConstants.Roles.User));
            Assert.True(userRoleResult.Succeeded, string.Join("; ", userRoleResult.Errors.Select(e => e.Description)));
        }

        var addToUserRoleResult = await userManager.AddToRoleAsync(user, ApplicationConstants.Roles.User);
        Assert.True(addToUserRoleResult.Succeeded, string.Join("; ", addToUserRoleResult.Errors.Select(e => e.Description)));

        var addToAdminRoleResult = await userManager.AddToRoleAsync(user, ApplicationConstants.Roles.Admin);
        Assert.True(addToAdminRoleResult.Succeeded, string.Join("; ", addToAdminRoleResult.Errors.Select(e => e.Description)));

        return (email, password, user.Id);
    }

    private static string ExtractAntiforgeryToken(string html)
    {
        var tokenMatch = Regex.Match(
            html,
            "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"",
            RegexOptions.Singleline);

        Assert.True(tokenMatch.Success, "Antiforgery token not found in HTML response.");
        return tokenMatch.Groups[1].Value;
    }
}
