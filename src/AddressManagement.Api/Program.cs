using AddressManagement.Application.Interfaces;
using AddressManagement.Application.Services;
using AddressManagement.Infrastructure.Persistence;
using AddressManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AddressManagement.Api.Middleware;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- Services registrieren ---

builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AddressDbContext>(options =>
    options.UseInMemoryDatabase("AddressManagementDb"));

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();
// --- JWT-Authentifizierung ---
var jwtKey = "ThisIsADemoSecretKeyForBuhlDataTask2026!"; // Nur für Demo-Zwecke - in Produktion aus Konfiguration/Secrets laden
var jwtIssuer = "AddressManagement.Api";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
    };
});

builder.Services.AddAuthorization();

// Damit AuthController den Key/Issuer nutzen kann, ohne sie zu duplizieren.
builder.Services.AddSingleton(new JwtSettings(jwtKey, jwtIssuer));

var app = builder.Build();

// --- Middleware-Pipeline ---
app.UseProblemDetailsExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Wird von WebApplicationFactory in den Integrationstests benötigt.
// Wird von WebApplicationFactory in den Integrationstests benötigt.
public partial class Program
{
}

public record JwtSettings(string Key, string Issuer);