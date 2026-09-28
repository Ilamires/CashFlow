namespace CashFlow.Api.Models;

public class Transaction
{
    public long Id { get; set; }

    public TransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "RUB";

    public string? Description { get; set; }

    public DateTimeOffset Date { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}