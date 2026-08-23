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
    [Fact]
    public async Task Get_WithExistingId_Returns200WithAddress()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/addresses", ValidCreateDto());
        var created = await createResponse.Content.ReadFromJsonAsync<AddressResponseDto>();

        // Act
        var response = await _client.GetAsync($"/api/addresses/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AddressResponseDto>();
        Assert.Equal(created.Id, body!.Id);
    }

    [Fact]
    public async Task Get_WithUnknownId_Returns404()
    {
        // Act
        var response = await _client.GetAsync($"/api/addresses/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithCityFilter_ReturnsOnlyMatchingAddresses()
    {
        // Arrange
        await _client.PostAsJsonAsync("/api/addresses", new AddressCreateDto
        {
            FirstName = "A",
            LastName = "B",
            Street = "S",
            HouseNumber = "1",
            PostalCode = "10000",
            City = "Hamburg",
            Country = "Germany",
        });
        await _client.PostAsJsonAsync("/api/addresses", new AddressCreateDto
        {
            FirstName = "C",
            LastName = "D",
            Street = "S",
            HouseNumber = "2",
            PostalCode = "20000",
            City = "Munich",
            Country = "Germany",
        });

        // Act
        var response = await _client.GetAsync("/api/addresses?city=Hamburg");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<AddressResponseDto>>();
        Assert.All(result!, address => Assert.Equal("Hamburg", address.City));
    }

    [Fact]
    public async Task Put_WithExistingId_Returns200AndUpdatesAddress()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/addresses", ValidCreateDto());
        var created = await createResponse.Content.ReadFromJsonAsync<AddressResponseDto>();

        var updateDto = new AddressUpdateDto
        {
            FirstName = "Updated",
            LastName = created!.LastName,
            Street = created.Street,
            HouseNumber = created.HouseNumber,
            PostalCode = created.PostalCode,
            City = created.City,
            Country = created.Country,
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/addresses/{created.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AddressResponseDto>();
        Assert.Equal("Updated", body!.FirstName);
    }

    [Fact]
    public async Task Put_WithUnknownId_Returns404()
    {
        // Arrange
        var updateDto = new AddressUpdateDto
        {
            FirstName = "X",
            LastName = "Y",
            Street = "S",
            HouseNumber = "1",
            PostalCode = "00000",
            City = "City",
            Country = "Germany",
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/addresses/{Guid.NewGuid()}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithExistingId_Returns204ThenSubsequentGetReturns404()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/addresses", ValidCreateDto());
        var created = await createResponse.Content.ReadFromJsonAsync<AddressResponseDto>();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/addresses/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/addresses/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithUnknownId_Returns404()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/addresses/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Patch_WithPartialData_Returns200AndUpdatesOnlyGivenFields()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync("/api/addresses", ValidCreateDto());
        var created = await createResponse.Content.ReadFromJsonAsync<AddressResponseDto>();

        var patchDto = new AddressPatchDto { City = "Hamburg" };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/addresses/{created!.Id}", patchDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AddressResponseDto>();
        Assert.Equal("Hamburg", body!.City);
        Assert.Equal(created.FirstName, body.FirstName); // unverändert
    }

    [Fact]
    public async Task Patch_WithUnknownId_Returns404()
    {
        // Act
        var response = await _client.PatchAsJsonAsync($"/api/addresses/{Guid.NewGuid()}", new AddressPatchDto());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}