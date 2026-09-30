using CashFlow.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Бизнес-ошибка: {Message}", ex.Message);
            await WriteProblemAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанное исключение");
            await WriteProblemAsync(context, 500, "Внутренняя ошибка сервера");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int status, string title)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Type = $"https://httpstatuses.io/{status}",
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}