using FluentValidation;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Domain.Extensions;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserValidator : AbstractValidator<RequestRegisterUserJson>
{
    public RegisterUserValidator()
    {
        RuleFor(user => user.Name).NotEmpty().WithMessage(ResourceMessagesExceptions.VALIDATION_NAME_REQUIRED);
        RuleFor(user => user.Email).NotEmpty().WithMessage(ResourceMessagesExceptions.VALIDATION_EMAIL_REQUIRED);
        RuleFor(user => user.Password).NotEmpty().WithMessage(ResourceMessagesExceptions.VALIDATION_PASSWORD_REQUIRED);
        When(user => user.Email.IsNotEmpty(), () =>
        {
            RuleFor(user => user.Email).EmailAddress().WithMessage(ResourceMessagesExceptions.VALIDATION_EMAIL_INVALID);
        });
    }
}