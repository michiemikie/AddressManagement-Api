namespace AddressManagement.Application.DTOs;

/// <summary>
/// Representation of an address returned to API clients.
///
/// SOLID: Single Responsibility Principle (SRP)
/// Represents exactly one thing: what a client sees when reading an
/// address. Kept separate from AddressCreateDto/AddressUpdateDto because
/// "what a client sends" and "what a client receives" are different
/// concerns — a response DTO always includes the generated Id, while
/// create/update DTOs never do.
/// </summary>
public class AddressResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string HouseNumber { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Email { get; set; }
}