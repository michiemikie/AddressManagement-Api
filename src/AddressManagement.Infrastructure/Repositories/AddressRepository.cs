using AddressManagement.Application.Interfaces;
using AddressManagement.Domain.Entities;
using AddressManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AddressManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IAddressRepository.
///
/// SOLID: Dependency Inversion Principle (DIP)
/// This class implements the abstraction defined in the Application layer
/// (IAddressRepository). Application never references this class directly —
/// it only knows the interface. This class could be swapped for a
/// different implementation (e.g. a SQL Server repository, or a fake one
/// for tests) without any change to Application or Api.
///
/// SOLID: Single Responsibility Principle (SRP)
/// This class' only job is translating repository operations into EF Core
/// calls. It contains no business logic (no validation, no DTO mapping) —
/// that all belongs to AddressService.
/// </summary>
public class AddressRepository : IAddressRepository
{
    private readonly AddressDbContext _context;

    public AddressRepository(AddressDbContext context)
    {
        _context = context;
    }

    public async Task<Address> AddAsync(Address address, CancellationToken cancellationToken = default)
    {
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);
        return address;
    }

    public async Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Address> Items, int TotalCount)> GetAllAsync(
    string? city,
    string? postalCode,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        var query = _context.Addresses.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(a => a.City.ToLower() == city.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(postalCode))
        {
            query = query.Where(a => a.PostalCode == postalCode);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
    public async Task UpdateAsync(Address address, CancellationToken cancellationToken = default)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Address address, CancellationToken cancellationToken = default)
    {
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);
    }
}