using Microsoft.EntityFrameworkCore;
using UserDirectory.Application.Common;
using UserDirectory.Domain.Entities;

namespace UserDirectory.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Age).IsRequired();
            entity.Property(x => x.City).HasMaxLength(100).IsRequired();
            entity.Property(x => x.State).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Pincode).HasMaxLength(10).IsRequired();
        });
    }
}
