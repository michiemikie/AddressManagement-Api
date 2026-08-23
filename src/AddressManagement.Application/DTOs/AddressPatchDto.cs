using System.ComponentModel.DataAnnotations;

namespace AddressManagement.Application.DTOs;

/// <summary>
/// Payload for a partial update (PATCH). All fields are optional -
/// only non-null fields are applied to the existing address.
/// </summary>
public class AddressPatchDto
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(150)]
    public string? Street { get; set; }

    [MaxLength(20)]
    public string? HouseNumber { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [EmailAddress, MaxLength(200)]
    public string? Email { get; set; }
}