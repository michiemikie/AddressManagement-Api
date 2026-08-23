using AddressManagement.Application.DTOs;

namespace AddressManagement.Application.Interfaces;

/// <summary>
/// Business logic for managing addresses. Consumed by the API layer.
///
/// SOLID: Dependency Inversion Principle (DIP)
/// The Api layer depends on this abstraction, not on the concrete
/// AddressService class. This means the controller doesn't need to know
/// HOW addresses are created/validated/persisted — only that something
/// implementing this interface can do it. Makes the controller trivially
/// testable and lets us swap the implementation later without touching
/// the Api layer.
///
/// SOLID: Interface Segregation Principle (ISP)
/// Exposes exactly the five operations the API needs (Create, Read single,
/// Read list, Update, Delete) — nothing more, nothing generic.
/// </summary>
public interface IAddressService
{
    Task<AddressResponseDto> CreateAsync(AddressCreateDto dto, CancellationToken cancellationToken = default);

    Task<AddressResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AddressResponseDto>> GetAllAsync(
        string? city,
        string? postalCode,
        CancellationToken cancellationToken = default);

    Task<AddressResponseDto?> UpdateAsync(Guid id, AddressUpdateDto dto, CancellationToken cancellationToken = default);

    Task<AddressResponseDto?> PatchAsync(Guid id, AddressPatchDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}