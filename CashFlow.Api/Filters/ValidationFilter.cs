using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CashFlow.Api.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (object? argument in context.ActionArguments.Values.Where(v => v is not null))
        {
            Type validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is IValidator validator)
            {
                ValidationResult result = await validator.ValidateAsync(new ValidationContext<object>(argument));
                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray());

                    context.Result = new BadRequestObjectResult(new ValidationProblemDetails(errors)
                    {
                        Status = 400,
                        Title = "Ошибка валидации запроса",
                    });
                    return;
                }
            }
        }

        await next();
    }
}