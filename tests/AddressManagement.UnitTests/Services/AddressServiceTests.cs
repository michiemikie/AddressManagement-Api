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
}