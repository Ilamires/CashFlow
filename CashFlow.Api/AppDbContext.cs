using CashFlow.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace CashFlow.Api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Email).HasMaxLength(255);
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.Property(a => a.Balance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.HasIndex(t => new { t.UserId, t.Date });
            e.HasCheckConstraint("CK_transactions_amount", "\"Amount\" > 0");
        });

        modelBuilder.Entity<Budget>(e =>
        {
            e.Property(b => b.LimitAmount).HasPrecision(18, 2);
            e.HasIndex(b => new { b.UserId, b.CategoryId, b.PeriodStart }).IsUnique();
            e.HasCheckConstraint("CK_budgets_period_start", "EXTRACT(DAY FROM \"PeriodStart\") = 1");
        });
    }
}