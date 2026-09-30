using CashFlow.Api.Contracts.Accounts;
using FluentValidation;

namespace CashFlow.Api.Validators;

public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название счёта обязательно")
            .MaximumLength(100).WithMessage("Название не длиннее 100 символов");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Валюта обязательна")
            .Length(3).WithMessage("Валюта - 3 символа")
            .Must(c => c.All(char.IsUpper)).WithMessage("Код валюты - заглавными буквами (RUB, USD)");
    }
}

public class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequest>
{
    public UpdateAccountRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название счёта обязательно")
            .MaximumLength(100).WithMessage("Название не длиннее 100 символов");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Валюта обязательна")
            .Length(3).WithMessage("Валюта - 3 символа")
            .Must(c => c.All(char.IsUpper)).WithMessage("Код валюты - заглавными буквами (RUB, USD)");
    }
}