using AddressManagement.Domain.Entities;

namespace AddressManagement.Application.Interfaces;

/// <summary>
/// Abstraction over persistence for <see cref="Address"/> entities.
/// Implemented in the Infrastructure layer using EF Core.
///
/// SOLID: Dependency Inversion Principle (DIP)
/// The Application layer defines this contract, but does NOT implement it.
/// Infrastructure depends on Application (by implementing this interface),
/// not the other way around. This means Application never needs to know
/// EF Core exists — it only knows "something that fulfils this interface".
/// That's what makes the Service layer testable without a real database.
///
/// SOLID: Interface Segregation Principle (ISP)
/// This interface only exposes the operations an Address repository
/// actually needs (no generic "repository of everything" abstraction),
/// keeping it focused and easy to mock in tests.
/// </summary>
public interface IAddressRepository
{
    Task<Address> AddAsync(Address address, CancellationToken cancellationToken = default);

    Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Address> Items, int TotalCount)> GetAllAsync(
     string? city,
     string? postalCode,
     int page,
     int pageSize,
     CancellationToken cancellationToken = default);

    Task UpdateAsync(Address address, CancellationToken cancellationToken = default);

    Task DeleteAsync(Address address, CancellationToken cancellationToken = default);
}