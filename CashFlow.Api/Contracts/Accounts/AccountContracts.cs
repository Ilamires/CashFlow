namespace CashFlow.Api.Contracts.Accounts;

public record CreateAccountRequest(string Name, string Currency = "RUB");

public record UpdateAccountRequest(string Name, string Currency);

public record AccountResponse(
    long Id,
    string Name,
    string Currency,
    decimal Balance,
    DateTimeOffset CreatedAt);