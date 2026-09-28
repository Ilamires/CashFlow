namespace CashFlow.Api.Models;

public class Budget
{
    public long Id { get; set; }

    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public DateOnly PeriodStart { get; set; }

    public decimal LimitAmount { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;
}