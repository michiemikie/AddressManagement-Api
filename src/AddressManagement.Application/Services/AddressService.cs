using AddressManagement.Application.DTOs;
using AddressManagement.Application.Interfaces;
using AddressManagement.Domain.Entities;
using System.Linq;

namespace AddressManagement.Application.Services;

/// <summary>
/// SOLID: Single Responsibility Principle (SRP)
/// This class' single job is address-related business logic (validation
/// orchestration, mapping between DTOs and the domain entity). It does NOT
/// know how persistence works — that's delegated entirely to
/// IAddressRepository.
///
/// SOLID: Dependency Inversion Principle (DIP)
/// The constructor depends on the IAddressRepository abstraction, not on
/// a concrete EF Core class. This is what lets us test this class with a
/// fake repository (see AddressServiceTests) instead of a real database.
/// </summary>
public class AddressService : IAddressService
{
    private readonly IAddressRepository _repository;

    public AddressService(IAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<AddressResponseDto> CreateAsync(AddressCreateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Address
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Street = dto.Street,
            HouseNumber = dto.HouseNumber,
            PostalCode = dto.PostalCode,
            City = dto.City,
            Country = dto.Country,
            Email = dto.Email,
        };

        var created = await _repository.AddAsync(entity, cancellationToken);

        return new AddressResponseDto
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Street = created.Street,
            HouseNumber = created.HouseNumber,
            PostalCode = created.PostalCode,
            City = created.City,
            Country = created.Country,
            Email = created.Email,
        };
    }

    // Die anderen Interface-Methoden (GetByIdAsync, GetAllAsync, UpdateAsync,
    // DeleteAsync) kommen im nächsten TDD-Zyklus - erst Test, dann Code.
    // Bis dahin würde der Compiler meckern, dass das Interface nicht
    // vollständig implementiert ist - das lösen wir gleich mit `throw new NotImplementedException()`.

    public async Task<AddressResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        return new AddressResponseDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Street = entity.Street,
            HouseNumber = entity.HouseNumber,
            PostalCode = entity.PostalCode,
            City = entity.City,
            Country = entity.Country,
            Email = entity.Email,
        };
    }


    public async Task<IReadOnlyList<AddressResponseDto>> GetAllAsync(string? city, string? postalCode, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(city, postalCode, cancellationToken);

        return entities
            .Select(entity => new AddressResponseDto
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Street = entity.Street,
                HouseNumber = entity.HouseNumber,
                PostalCode = entity.PostalCode,
                City = entity.City,
                Country = entity.Country,
                Email = entity.Email,
            })
            .ToList();
    }

    public Task<AddressResponseDto?> UpdateAsync(Guid id, AddressUpdateDto dto, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

}