namespace CashFlow.Api.Models;

public enum TransactionType
{
    Income,
    Expense
}
public class Category
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TransactionType Type { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public List<Transaction> Transactions { get; set; } = [];
    public List<Budget> Budgets { get; set; } = [];
}