using CashFlow.Api.Contracts.Transactions;
using FluentValidation;

namespace CashFlow.Api.Validators;

public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
{
    private static readonly DateTimeOffset MinDate = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public CreateTransactionRequestValidator()
    {
        RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Не указан счёт");
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Не указана категория");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Некорректный тип транзакции");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Сумма должна быть больше нуля");
        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(MinDate)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(5))
            .WithMessage("Дата вне допустимого диапазона или в будущем");
        RuleFor(x => x.Description)
            .MaximumLength(1000).When(x => x.Description is not null);
    }
}

public class UpdateTransactionRequestValidator : AbstractValidator<UpdateTransactionRequest>
{
    private static readonly DateTimeOffset MinDate = new(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public UpdateTransactionRequestValidator()
    {
        RuleFor(x => x.AccountId).GreaterThan(0).WithMessage("Не указан счёт");
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Не указана категория");
        RuleFor(x => x.Type).IsInEnum().WithMessage("Некорректный тип транзакции");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Сумма должна быть больше нуля");
        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(MinDate)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow.AddMinutes(5))
            .WithMessage("Дата вне допустимого диапазона или в будущем");
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description is not null);
    }
}

public class TransactionQueryRequestValidator : AbstractValidator<TransactionQueryRequest>
{
    public TransactionQueryRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.From == null || x.To == null || x.To >= x.From)
            .WithMessage("'To' не может быть раньше 'From'");
    }
}