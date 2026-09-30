using CashFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Api.Data;

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
            e.Property(u => u.FirstName).HasMaxLength(100);
            e.Property(u => u.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.Property(a => a.Balance).HasPrecision(18, 2);
            e.Property(a => a.Name).HasMaxLength(100);
            e.Property(a => a.Currency).HasMaxLength(3);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.Property(t => t.Currency).HasMaxLength(3);
            e.Property(t => t.Description).HasMaxLength(1000);
            e.HasIndex(t => new { t.UserId, t.Date });
            e.HasCheckConstraint("CK_transactions_amount", "\"Amount\" > 0");
            e.HasCheckConstraint("CK_transactions_date",
                "\"Date\" >= '1970-01-01 00:00:00+00'::timestamptz " +
                "AND \"Date\" < '2300-01-01 00:00:00+00'::timestamptz");
            e.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Budget>(e =>
        {
            e.Property(b => b.LimitAmount).HasPrecision(18, 2);
            e.HasIndex(b => new { b.UserId, b.CategoryId, b.PeriodStart }).IsUnique();
            e.HasCheckConstraint("CK_budgets_period_start", "EXTRACT(DAY FROM \"PeriodStart\") = 1");
            e.HasCheckConstraint("CK_budgets_period_range",
                "\"PeriodStart\" >= DATE '1970-01-01' AND \"PeriodStart\" < DATE '2300-01-01'");
            e.HasOne(b => b.Category)
                .WithMany(c => c.Budgets)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}