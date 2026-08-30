using AddressManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AddressManagement.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for address persistence.
///
/// SOLID: Single Responsibility Principle (SRP)
/// This class' only job is describing how the domain model maps to
/// storage (table shape, constraints, indexes). It contains no business
/// logic — that belongs in AddressService.
/// </summary>
public class AddressDbContext : DbContext
{
    public AddressDbContext(DbContextOptions<AddressDbContext> options) : base(options)
    {
    }

    public DbSet<Address> Addresses => Set<Address>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(builder =>
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.LastName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Street).IsRequired().HasMaxLength(150);
            builder.Property(a => a.HouseNumber).IsRequired().HasMaxLength(20);
            builder.Property(a => a.PostalCode).IsRequired().HasMaxLength(20);
            builder.Property(a => a.City).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Country).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Email).HasMaxLength(200);
            builder.Property(a => a.RowVersion).IsRowVersion();

            builder.HasIndex(a => a.City);
            builder.HasIndex(a => a.PostalCode);
        });
    }
}