namespace AddressManagement.Application.Exceptions;

/// <summary>
/// Thrown when an update or delete operation conflicts with a concurrent
/// modification (RowVersion mismatch). Translated to HTTP 409 by the
/// controller/middleware.
/// </summary>
public class ConcurrencyConflictException : Exception
{
    public ConcurrencyConflictException(string message) : base(message)
    {
    }
}