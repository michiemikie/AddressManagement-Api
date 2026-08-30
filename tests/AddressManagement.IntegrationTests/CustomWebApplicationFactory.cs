using System.Net.Http.Headers;
using System.Net.Http.Json;
using AddressManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AddressManagement.IntegrationTests;

/// <summary>
/// Spins up the API in-memory for integration tests. Each factory instance
/// gets its own uniquely named database so tests don't leak state between
/// each other.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AddressDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AddressDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    /// <summary>
    /// Creates an HttpClient that automatically logs in with the demo user
    /// and attaches a valid JWT to every request, so tests don't need to
    /// repeat the login flow themselves.
    /// </summary>
    public async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Username = "admin",
            Password = "password123",
        });

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult!.Token);

        return client;
    }

    private record LoginResult(string Token, DateTime ExpiresAt);
}