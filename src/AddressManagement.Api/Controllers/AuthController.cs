using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AddressManagement.Api.Controllers;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, DateTime ExpiresAt);

/// <summary>
/// Minimal login endpoint for demo purposes. Issues a signed JWT for a
/// single hardcoded demo user - a real system would validate against a
/// user store with hashed passwords.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtSettings _jwtSettings;

    // Demo-Nutzer - in einer echten Anwendung würde man gegen eine
    // Nutzer-Datenbank mit gehashten Passwörtern prüfen.
    private const string DemoUsername = "admin";
    private const string DemoPassword = "password123";

    public AuthController(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    /// <summary>Issues a JWT for the demo user.</summary>
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        if (request.Username != DemoUsername || request.Password != DemoPassword)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid credentials",
                Status = StatusCodes.Status401Unauthorized,
            });
        }

        var expiresAt = DateTime.UtcNow.AddHours(1);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new LoginResponse(tokenString, expiresAt));
    }
}