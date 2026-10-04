using FluentValidation.Results;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _updateOnlyRepository;

    public ChangePasswordUseCase(
        IPasswordHasher passwordHasher,
        ILoggedUser loggedUser,
        IUserUpdateOnlyRepository updateOnlyRepository)
    {
        _passwordHasher = passwordHasher;
        _loggedUser = loggedUser;
        _updateOnlyRepository = updateOnlyRepository;
    }

    public async Task Execute(RequestChangePasswordJson request)
    {
        var loggedUser = await _loggedUser.Get();

        Validate(request, loggedUser);

        var hashPassword = _passwordHasher.HashPassword(request.NewPassword);

        await _updateOnlyRepository.UpdatePassword(loggedUser.Id, hashPassword);
    }

    private void Validate(RequestChangePasswordJson request, Domain.Entities.User user)
    {
        var validator = new ChangePasswordValidator();

        var res = validator.Validate(request);

        if (_passwordHasher.VerifyPassword(request.CurrentPassword, user.Password) == false)
            res.Errors.Add(
                new ValidationFailure(string.Empty, ResourceMessagesExceptions.VALIDATION_CURRENT_PASSWORD)
            );

        if (res.IsValid == false)
            throw new ErrorOnValidationException(res.Errors.Select(error => error.ErrorMessage).ToList());
    }
}