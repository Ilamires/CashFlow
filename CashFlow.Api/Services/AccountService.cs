using CashFlow.Api.Contracts.Accounts;
using CashFlow.Api.Data;
using CashFlow.Api.Exceptions;
using CashFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Api.Services;

public interface IAccountService
{
    Task<AccountResponse> CreateAsync(long userId, CreateAccountRequest request);
    Task<IReadOnlyList<AccountResponse>> GetAllAsync(long userId);
    Task<AccountResponse> GetByIdAsync(long userId, long id);
    Task<AccountResponse> UpdateAsync(long userId, long id, UpdateAccountRequest request);
    Task DeleteAsync(long userId, long id);
}

public class AccountService(AppDbContext db) : IAccountService
{
    public async Task<AccountResponse> CreateAsync(long userId, CreateAccountRequest request)
    {
        var duplicate = await db.Accounts.AnyAsync(a => a.UserId == userId && a.Name == request.Name);
        if (duplicate)
            throw new ConflictException($"Счёт «{request.Name}» уже существует");

        var account = new Account
        {
            UserId = userId,
            Name = request.Name,
            Currency = request.Currency,
            Balance = 0,
        };

        db.Accounts.Add(account);
        await db.SaveChangesAsync();
        return ToResponse(account);
    }

    public async Task<IReadOnlyList<AccountResponse>> GetAllAsync(long userId)
        => await db.Accounts
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.Name)
            .Select(a => ToResponse(a))
            .ToListAsync();

    public async Task<AccountResponse> GetByIdAsync(long userId, long id)
    {
        var account = await db.Accounts.SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (account is null)
            throw new NotFoundException("Счёт не найден");

        return ToResponse(account);
    }

    public async Task<AccountResponse> UpdateAsync(long userId, long id, UpdateAccountRequest request)
    {
        var account = await db.Accounts.SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (account is null)
            throw new NotFoundException("Счёт не найден");

        account.Name = request.Name;
        account.Currency = request.Currency;

        await db.SaveChangesAsync();
        return ToResponse(account);
    }

    public async Task DeleteAsync(long userId, long id)
    {
        var account = await db.Accounts.SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (account is null)
            throw new NotFoundException("Счёт не найден");

        var hasTransactions = await db.Transactions.AnyAsync(t => t.AccountId == id);
        if (hasTransactions)
            throw new ConflictException("Нельзя удалить счёт: по нему есть транзакции");

        db.Accounts.Remove(account);
        await db.SaveChangesAsync();
    }
    private static AccountResponse ToResponse(Account account)
        => new(account.Id, account.Name, account.Currency, account.Balance, account.CreatedAt);
}