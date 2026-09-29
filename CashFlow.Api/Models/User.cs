namespace CashFlow.Api.Models;

public class User
{
    public long Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<Account> Accounts { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
    public List<Transaction> Transactions { get; set; } = [];
    public List<Budget> Budgets { get; set; } = [];
}