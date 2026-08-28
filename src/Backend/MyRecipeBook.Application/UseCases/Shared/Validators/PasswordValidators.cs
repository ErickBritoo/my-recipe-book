using FluentValidation;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.UseCases.Shared.Validators;

public static class PasswordValidators
{
    extension<TRequest>(IRuleBuilderInitial<TRequest, string> ruleBuilder)
    {
        public IRuleBuilderOptions<TRequest, string> Password()
        {
            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage(ResourceMessagesExceptions.VALIDATION_PASSWORD_REQUIRED)
                .MinimumLength(6).WithMessage(ResourceMessagesExceptions.VALIDATION_PASSWORD_MIN_LENGTH);
        }
    }
}