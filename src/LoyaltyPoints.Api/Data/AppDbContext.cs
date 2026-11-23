using LoyaltyPoints.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LoyaltyPoints.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PointTransaction> PointTransactions => Set<PointTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aynı e-postayla iki müşteri olmasın
        modelBuilder.Entity<Customer>().HasIndex(c => c.Email).IsUnique();
    }
}