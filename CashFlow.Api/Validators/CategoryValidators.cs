using CashFlow.Api.Contracts.Categories;
using FluentValidation;

namespace CashFlow.Api.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название категории обязательно")
            .MaximumLength(100).WithMessage("Название не длиннее 100 символов");

        RuleFor(x => x.Type).IsInEnum().WithMessage("Некорректный тип категории");
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название категории обязательно")
            .MaximumLength(100).WithMessage("Название не длиннее 100 символов");

        RuleFor(x => x.Type).IsInEnum().WithMessage("Некорректный тип категории");
    }
}