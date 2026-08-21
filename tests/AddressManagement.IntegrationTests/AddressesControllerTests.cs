using System.Net;
using System.Net.Http.Json;
using AddressManagement.Application.DTOs;
using Xunit;

namespace AddressManagement.IntegrationTests;

public class AddressesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AddressesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static AddressCreateDto ValidCreateDto() => new()
    {
        FirstName = "Max",
        LastName = "Mustermann",
        Street = "Musterstraße",
        HouseNumber = "12a",
        PostalCode = "35576",
        City = "Wetzlar",
        Country = "Germany",
        Email = "max@example.com",
    };

    [Fact]
    public async Task Post_WithValidAddress_Returns201WithLocationHeader()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/addresses", ValidCreateDto());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var body = await response.Content.ReadFromJsonAsync<AddressResponseDto>();
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body!.Id);
    }
}