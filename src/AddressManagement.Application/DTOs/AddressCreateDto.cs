using System.ComponentModel.DataAnnotations;

namespace AddressManagement.Application.DTOs;

/// <summary>
/// Payload for creating a new address.
///
/// SOLID: Single Responsibility Principle (SRP)
/// This class has exactly one job – describing what a client must send
/// to create an address. It's intentionally separate from the Address
/// entity and from AddressUpdateDto, even though the fields overlap:
/// a "create" request and an "update" request represent different
/// operations and may evolve independently (e.g. Create could later
/// require a duplicate-check field that Update never needs).
/// </summary>
public class AddressCreateDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Street { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string HouseNumber { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }
}