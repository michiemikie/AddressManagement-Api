using AddressManagement.Application.Interfaces;
using AddressManagement.Application.Services;
using AddressManagement.Infrastructure.Persistence;
using AddressManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AddressManagement.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- Services registrieren ---

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AddressDbContext>(options =>
    options.UseInMemoryDatabase("AddressManagementDb"));

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();

var app = builder.Build();

// --- Middleware-Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Wird von WebApplicationFactory in den Integrationstests benötigt.
public partial class Program
{
}