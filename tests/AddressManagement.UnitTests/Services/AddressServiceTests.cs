using AddressManagement.Application.DTOs;
using AddressManagement.Application.Interfaces;
using AddressManagement.Application.Services;
using AddressManagement.Domain.Entities;
using NSubstitute;
using Xunit;

namespace AddressManagement.UnitTests.Services;

public class AddressServiceTests
{
    private readonly IAddressRepository _repository;
    private readonly AddressService _sut; // sut = "system under test"

    public AddressServiceTests()
    {
        _repository = Substitute.For<IAddressRepository>();
        _sut = new AddressService(_repository);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedAddressWithGeneratedId()
    {
        // Arrange
        var dto = new AddressCreateDto
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

        _repository.AddAsync(Arg.Any<Address>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Address>()));

        // Act
        var result = await _sut.CreateAsync(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(dto.FirstName, result.FirstName);
        Assert.Equal(dto.LastName, result.LastName);
    }
    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsMappedAddress()
    {
        // Arrange
        var address = new Address
        {
            Id = Guid.NewGuid(),
            FirstName = "Erika",
            LastName = "Musterfrau",
            Street = "Beispielweg",
            HouseNumber = "5",
            PostalCode = "10115",
            City = "Berlin",
            Country = "Germany",
        };
        _repository.GetByIdAsync(address.Id, Arg.Any<CancellationToken>()).Returns(address);

        // Act
        var result = await _sut.GetByIdAsync(address.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(address.Id, result!.Id);
        Assert.Equal("Berlin", result.City);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ReturnsNull()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Address?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
    [Fact]
    public async Task GetAllAsync_WithCityFilter_ReturnsOnlyMatchingAddresses()
    {
        // Arrange
        var berlinAddress = new Address { Id = Guid.NewGuid(), City = "Berlin" };

        _repository.GetAllAsync("Berlin", null, Arg.Any<CancellationToken>())
            .Returns(new List<Address> { berlinAddress });

        // Act
        var result = await _sut.GetAllAsync(city: "Berlin", postalCode: null);

        // Assert
        Assert.Single(result);
        Assert.Equal("Berlin", result[0].City);
    }

    [Fact]
    public async Task GetAllAsync_WithoutFilter_PassesNullFiltersToRepository()
    {
        // Arrange
        _repository.GetAllAsync(null, null, Arg.Any<CancellationToken>())
            .Returns(new List<Address>());

        // Act
        var result = await _sut.GetAllAsync(city: null, postalCode: null);

        // Assert
        Assert.Empty(result);
        await _repository.Received(1).GetAllAsync(null, null, Arg.Any<CancellationToken>());
    }
    [Fact]
    public async Task UpdateAsync_WithExistingId_UpdatesAndReturnsAddress()
    {
        // Arrange
        var existing = new Address
        {
            Id = Guid.NewGuid(),
            FirstName = "Old",
            LastName = "Name",
            Street = "Old Street",
            HouseNumber = "1",
            PostalCode = "00000",
            City = "Old City",
            Country = "Germany",
        };
        _repository.GetByIdAsync(existing.Id, Arg.Any<CancellationToken>()).Returns(existing);

        var dto = new AddressUpdateDto
        {
            FirstName = "New",
            LastName = "Name",
            Street = "New Street",
            HouseNumber = "2",
            PostalCode = "11111",
            City = "New City",
            Country = "Germany",
        };

        // Act
        var result = await _sut.UpdateAsync(existing.Id, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New", result!.FirstName);
        Assert.Equal("New City", result.City);
        await _repository.Received(1).UpdateAsync(Arg.Any<Address>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ReturnsNullAndDoesNotCallRepositoryUpdate()
    {
        // Arrange
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Address?)null);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), new AddressUpdateDto());

        // Assert
        Assert.Null(result);
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Address>(), Arg.Any<CancellationToken>());
    }
}