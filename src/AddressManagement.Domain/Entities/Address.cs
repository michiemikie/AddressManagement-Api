using System;
using System.Collections.Generic;
using System.Text;

namespace AddressManagement.Domain.Entities;

/// <summary>
/// Represents a postal address record.
/// 
/// SOLID: Single Responsibility Principle (SRP)
/// This class has exactly one responsibility – holding address data.
/// It contains no persistence logic (that's Infrastructure), no validation
/// rules beyond its own data shape (that's Application/DTOs), and no
/// knowledge of HTTP or the API (that's the Api layer). If this class ever
/// needs to change, it should only be because the definition of "what is
/// an address" has changed – not because the database or the API changed.
/// </summary>
public class Address
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string HouseNumber { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    /// <summary>Optional contact email.</summary>
    public string? Email { get; set; }
}