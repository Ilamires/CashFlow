namespace CashFlow.Api.Models;

public class Account
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Currency { get; set; } = "RUB";

    public decimal Balance { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public List<Transaction> Transactions { get; set; } = [];
}