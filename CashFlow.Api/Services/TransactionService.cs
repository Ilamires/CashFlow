using CashFlow.Api.Contracts.Transactions;
using CashFlow.Api.Data;
using CashFlow.Api.Exceptions;
using CashFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Api.Services;

public interface ITransactionService
{
    Task<TransactionResponse> CreateAsync(long userId, CreateTransactionRequest request);
    Task<IReadOnlyList<TransactionResponse>> GetAllAsync(long userId, TransactionQueryRequest query);
    Task<TransactionResponse> GetByIdAsync(long userId, long id);
    Task<TransactionResponse> UpdateAsync(long userId, long id, UpdateTransactionRequest request);
    Task DeleteAsync(long userId, long id);
}

public class TransactionService(AppDbContext db) : ITransactionService
{
    public async Task<TransactionResponse> CreateAsync(long userId, CreateTransactionRequest request)
    {
        var account = await db.Accounts.SingleOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId);
        if (account is null)
            throw new NotFoundException("Счёт не найден");

        var category = await db.Categories.SingleOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId);
        if (category is null)
            throw new NotFoundException("Категория не найдена");

        EnsureTypeMatches(category, request.Type);
        EnsureFundsAvailable(account, request.Type, request.Amount);

        var transaction = new Transaction
        {
            UserId = userId,
            AccountId = account.Id,
            CategoryId = category.Id,
            Type = request.Type,
            Amount = request.Amount,
            Currency = account.Currency,
            Description = request.Description,
            Date = request.Date,
        };

        ApplyToBalance(account, request.Type, request.Amount);
        db.Transactions.Add(transaction);

        await db.SaveChangesAsync();

        return ToResponse(transaction, account, category);
    }

    public async Task<IReadOnlyList<TransactionResponse>> GetAllAsync(long userId, TransactionQueryRequest query)
        => await db.Transactions
            .Where(t => t.UserId == userId)
            .Where(t => query.AccountId == null || t.AccountId == query.AccountId)
            .Where(t => query.CategoryId == null || t.CategoryId == query.CategoryId)
            .Where(t => query.Type == null || t.Type == query.Type)
            .Where(t => query.From == null || t.Date >= query.From)
            .Where(t => query.To == null || t.Date < query.To)
            .OrderByDescending(t => t.Date)
            .Select(t => new TransactionResponse(
                t.Id, t.AccountId, t.Account.Name, t.CategoryId, t.Category.Name,
                t.Type, t.Amount, t.Currency, t.Description, t.Date))
            .ToListAsync();

    public async Task<TransactionResponse> GetByIdAsync(long userId, long id)
    {
        var response = await db.Transactions
            .Where(t => t.Id == id && t.UserId == userId)
            .Select(t => new TransactionResponse(
                t.Id, t.AccountId, t.Account.Name, t.CategoryId, t.Category.Name,
                t.Type, t.Amount, t.Currency, t.Description, t.Date))
            .SingleOrDefaultAsync();

        if (response is null)
            throw new NotFoundException("Транзакция не найдена");

        return response;
    }

    public async Task<TransactionResponse> UpdateAsync(long userId, long id, UpdateTransactionRequest request)
    {
        var transaction = await db.Transactions.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (transaction is null)
            throw new NotFoundException("Транзакция не найдена");

        var newAccount = await db.Accounts.SingleOrDefaultAsync(a => a.Id == request.AccountId && a.UserId == userId);
        if (newAccount is null)
            throw new NotFoundException("Счёт не найден");

        var newCategory = await db.Categories.SingleOrDefaultAsync(c => c.Id == request.CategoryId && c.UserId == userId);
        if (newCategory is null)
            throw new NotFoundException("Категория не найдена");

        EnsureTypeMatches(newCategory, request.Type);

        var oldAccount = newAccount.Id == transaction.AccountId
            ? newAccount
            : await db.Accounts.SingleOrDefaultAsync(a => a.Id == transaction.AccountId && a.UserId == userId);
        if (oldAccount is null)
            throw new NotFoundException("Счёт транзакции не найден");

        ReverseBalance(oldAccount, transaction.Type, transaction.Amount);

        EnsureFundsAvailable(newAccount, request.Type, request.Amount);
        ApplyToBalance(newAccount, request.Type, request.Amount);

        transaction.AccountId = newAccount.Id;
        transaction.CategoryId = newCategory.Id;
        transaction.Type = request.Type;
        transaction.Amount = request.Amount;
        transaction.Currency = newAccount.Currency;
        transaction.Description = request.Description;
        transaction.Date = request.Date;

        await db.SaveChangesAsync();

        return ToResponse(transaction, newAccount, newCategory);
    }

    public async Task DeleteAsync(long userId, long id)
    {
        var transaction = await db.Transactions.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (transaction is null)
            throw new NotFoundException("Транзакция не найдена");

        var account = await db.Accounts.SingleOrDefaultAsync(a => a.Id == transaction.AccountId && a.UserId == userId);
        if (account is null)
            throw new NotFoundException("Счёт транзакции не найден");

        ReverseBalance(account, transaction.Type, transaction.Amount);
        db.Transactions.Remove(transaction);

        await db.SaveChangesAsync();
    }

    private static void EnsureTypeMatches(Category category, TransactionType type)
    {
        if (category.Type != type)
            throw new BusinessRuleException("Тип транзакции не совпадает с типом категории");
    }

    private static void EnsureFundsAvailable(Account account, TransactionType type, decimal amount)
    {
        if (type == TransactionType.Expense && account.Balance < amount)
            throw new BusinessRuleException("Недостаточно средств на счёте");
    }

    private static void ApplyToBalance(Account account, TransactionType type, decimal amount)
    {
        account.Balance = type == TransactionType.Income
            ? account.Balance + amount
            : account.Balance - amount;
    }

    private static void ReverseBalance(Account account, TransactionType type, decimal amount)
    {
        account.Balance = type == TransactionType.Income
            ? account.Balance - amount
            : account.Balance + amount;
    }

    private static TransactionResponse ToResponse(Transaction t, Account account, Category category)
        => new(t.Id, account.Id, account.Name, category.Id, category.Name,
            t.Type, t.Amount, t.Currency, t.Description, t.Date);
}