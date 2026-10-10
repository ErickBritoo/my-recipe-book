using FluentValidation;
using MyRecipeBook.Application.UseCases.Shared.Validators;
using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<RequestResetRecoveryPasswordJson>
{
    public ResetPasswordValidator()
    {
        RuleFor(request => request.NewPassword).Password();
    }
}