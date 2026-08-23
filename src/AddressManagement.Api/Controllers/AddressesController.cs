using AddressManagement.Application.DTOs;
using AddressManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AddressManagement.Api.Controllers;

[ApiController]
[Route("api/addresses")]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    /// <summary>Creates a new address.</summary>
    [HttpPost]
    public async Task<ActionResult<AddressResponseDto>> Create(
        [FromBody] AddressCreateDto dto,
        CancellationToken cancellationToken)
    {
        var created = await _addressService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Retrieves a single address by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AddressResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var address = await _addressService.GetByIdAsync(id, cancellationToken);
        return address is null ? NotFound() : Ok(address);
    }
    /// <summary>Lists addresses, optionally filtered by city and/or postal code.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AddressResponseDto>>> GetAll(
        [FromQuery] string? city,
        [FromQuery] string? postalCode,
        CancellationToken cancellationToken)
    {
        var addresses = await _addressService.GetAllAsync(city, postalCode, cancellationToken);
        return Ok(addresses);
    }

    /// <summary>Fully replaces an existing address.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AddressResponseDto>> Update(
        Guid id,
        [FromBody] AddressUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _addressService.UpdateAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Partially updates an existing address.</summary>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<AddressResponseDto>> Patch(
        Guid id,
        [FromBody] AddressPatchDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _addressService.PatchAsync(id, dto, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Deletes an address.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _addressService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}