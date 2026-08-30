using System.ComponentModel.DataAnnotations;

namespace AddressManagement.Application.DTOs;

/// <summary>
/// Payload for a full update (PUT) of an existing address.
///
/// SOLID: Single Responsibility Principle (SRP)
/// Represents exactly one thing: the complete set of fields required to
/// fully replace an existing address (PUT semantics — every field is
/// required, unlike a PATCH which would allow partial updates).
/// </summary>
public class AddressUpdateDto
{
    /// <summary>
    /// Base64-encoded concurrency token, taken from a previous GET/POST response.
    /// If provided, the update fails with 409 Conflict if the address was
    /// modified since this token was issued.
    /// </summary>
    public string? RowVersion { get; set; }
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