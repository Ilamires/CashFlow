using CashFlow.Api.Models;

namespace CashFlow.Api.Contracts.Categories;

public record CreateCategoryRequest(string Name, TransactionType Type);

public record UpdateCategoryRequest(string Name, TransactionType Type);

public record CategoryResponse(long Id, string Name, TransactionType Type);