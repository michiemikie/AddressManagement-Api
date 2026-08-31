using AddressManagement.Application.DTOs;
using AddressManagement.Application.Interfaces;
using AddressManagement.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
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
    private readonly IMemoryCache _cache;

    public AddressService(IAddressRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
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
            RowVersion = created.RowVersion is null ? null : Convert.ToBase64String(created.RowVersion),
        };

    }



    // Die anderen Interface-Methoden (GetByIdAsync, GetAllAsync, UpdateAsync,
    // DeleteAsync) kommen im nächsten TDD-Zyklus - erst Test, dann Code.
    // Bis dahin würde der Compiler meckern, dass das Interface nicht
    // vollständig implementiert ist - das lösen wir gleich mit `throw new NotImplementedException()`.

    public async Task<AddressResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"address_{id}";

        if (_cache.TryGetValue(cacheKey, out AddressResponseDto? cached))
        {
            return cached;
        }

        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var dto = new AddressResponseDto
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
            RowVersion = entity.RowVersion is null ? null : Convert.ToBase64String(entity.RowVersion),
        };

        _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));

        return dto;
    }

    public async Task<PagedResultDto<AddressResponseDto>> GetAllAsync(
    string? city,
    string? postalCode,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        // Defensive Grenzen: falls der Client ungültige Werte schickt, nutzen
        // wir sinnvolle Standardwerte statt einen Fehler zu werfen.
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var (entities, totalCount) = await _repository.GetAllAsync(city, postalCode, page, pageSize, cancellationToken);

        return new PagedResultDto<AddressResponseDto>
        {
            Items = entities
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
                    RowVersion = entity.RowVersion is null ? null : Convert.ToBase64String(entity.RowVersion),

                })
                .ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }
    /// <summary>
    /// SOLID: Open/Closed Principle (OCP) — in spirit
    /// This method mutates the existing tracked entity rather than
    /// replacing it wholesale. If we later add a field (e.g. an audit
    /// timestamp), this update logic can be extended without breaking
    /// the "identity" of the entity (same object reference, same Id).
    /// </summary>
    public async Task<AddressResponseDto?> UpdateAsync(Guid id, AddressUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        // Falls der Client einen RowVersion-Token mitschickt, übernehmen wir ihn
        // als "Original-Wert" - das ist es, was EF Core beim Speichern mit dem
        // aktuellen DB-Stand vergleicht, um Concurrency-Konflikte zu erkennen.
        if (!string.IsNullOrEmpty(dto.RowVersion))
        {
            entity.RowVersion = Convert.FromBase64String(dto.RowVersion);
        }

        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Street = dto.Street;
        entity.HouseNumber = dto.HouseNumber;
        entity.PostalCode = dto.PostalCode;
        entity.City = dto.City;
        entity.Country = dto.Country;
        entity.Email = dto.Email;

        await _repository.UpdateAsync(entity, cancellationToken);

        _cache.Remove($"address_{id}");

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
            RowVersion = entity.RowVersion is null ? null : Convert.ToBase64String(entity.RowVersion),
        };
    }
    public async Task<AddressResponseDto?> PatchAsync(Guid id, AddressPatchDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        if (dto.FirstName is not null) entity.FirstName = dto.FirstName;
        if (dto.LastName is not null) entity.LastName = dto.LastName;
        if (dto.Street is not null) entity.Street = dto.Street;
        if (dto.HouseNumber is not null) entity.HouseNumber = dto.HouseNumber;
        if (dto.PostalCode is not null) entity.PostalCode = dto.PostalCode;
        if (dto.City is not null) entity.City = dto.City;
        if (dto.Country is not null) entity.Country = dto.Country;
        if (dto.Email is not null) entity.Email = dto.Email;

        await _repository.UpdateAsync(entity, cancellationToken);

        _cache.Remove($"address_{id}");

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
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteAsync(entity, cancellationToken);

        _cache.Remove($"address_{id}");

        return true;
    }
   
}