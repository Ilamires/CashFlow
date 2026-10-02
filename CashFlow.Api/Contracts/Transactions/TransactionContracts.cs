using CashFlow.Api.Models;

namespace CashFlow.Api.Contracts.Transactions;

public record CreateTransactionRequest(
    long AccountId,
    long CategoryId,
    TransactionType Type,
    decimal Amount,
    DateTimeOffset Date,
    string? Description = null);

public record UpdateTransactionRequest(
    long AccountId,
    long CategoryId,
    TransactionType Type,
    decimal Amount,
    DateTimeOffset Date,
    string? Description);

public record TransactionQueryRequest(
    long? AccountId = null,
    long? CategoryId = null,
    TransactionType? Type = null,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null);

public record TransactionResponse(
    long Id,
    long AccountId,
    string AccountName,
    long CategoryId,
    string CategoryName,
    TransactionType Type,
    decimal Amount,
    string Currency,
    string? Description,
    DateTimeOffset Date);