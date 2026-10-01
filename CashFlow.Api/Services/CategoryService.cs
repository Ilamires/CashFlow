using CashFlow.Api.Contracts.Categories;
using CashFlow.Api.Data;
using CashFlow.Api.Exceptions;
using CashFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Api.Services;

public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(long userId, CreateCategoryRequest request);
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(long userId, TransactionType? type);
    Task<CategoryResponse> UpdateAsync(long userId, long id, UpdateCategoryRequest request);
    Task DeleteAsync(long userId, long id);
}

public class CategoryService(AppDbContext db) : ICategoryService
{
    public async Task<CategoryResponse> CreateAsync(long userId, CreateCategoryRequest request)
    {
        var duplicate = await db.Categories.AnyAsync(c =>
            c.UserId == userId && c.Name == request.Name && c.Type == request.Type);
        if (duplicate)
            throw new ConflictException($"Категория «{request.Name}» этого типа уже существует");

        var category = new Category
        {
            UserId = userId,
            Name = request.Name,
            Type = request.Type,
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return ToResponse(category);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(long userId, TransactionType? type)
        => await db.Categories
            .Where(c => c.UserId == userId && (type == null || c.Type == type))
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Type))
            .ToListAsync();

    public async Task<CategoryResponse> UpdateAsync(long userId, long id, UpdateCategoryRequest request)
    {
        var category = await db.Categories.SingleOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (category is null)
            throw new NotFoundException("Категория не найдена");

        category.Name = request.Name;
        category.Type = request.Type;

        await db.SaveChangesAsync();
        return ToResponse(category);
    }

    public async Task DeleteAsync(long userId, long id)
    {
        var category = await db.Categories.SingleOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (category is null)
            throw new NotFoundException("Категория не найдена");

        var hasTransactions = await db.Transactions.AnyAsync(t => t.CategoryId == id);
        var hasBudgets = await db.Budgets.AnyAsync(b => b.CategoryId == id);
        if (hasTransactions || hasBudgets)
            throw new ConflictException("Нельзя удалить категорию: она используется в транзакциях или бюджетах");

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
    }

    private static CategoryResponse ToResponse(Category category)
        => new(category.Id, category.Name, category.Type);
}