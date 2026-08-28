using FluentValidation.Results;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Update;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
    private readonly IUnityOfWork _unityOfWork;
    
    
    public UpdateUserUseCase(
        ILoggedUser loggedUser,
        IUserReadOnlyRepository userReadOnlyRepository,
        IUserUpdateOnlyRepository updateOnlyRepository,
        IUnityOfWork unityOfWork)
    {
        _loggedUser = loggedUser;
        _userReadOnlyRepository = userReadOnlyRepository;
        _userUpdateOnlyRepository = updateOnlyRepository;
        _unityOfWork = unityOfWork;
    }
    
    public async Task Execute(RequestUpdateUserJson request)
    {
        var loggedUser = await _loggedUser.Get();
        
        await Validate(request, loggedUser);

        loggedUser.Email = request.Email;
        loggedUser.Name = request.Name;
        
        _userUpdateOnlyRepository.UpdateProfile(loggedUser);

        await _unityOfWork.Commit();
    }

    private async Task Validate(RequestUpdateUserJson request, Domain.Entities.User loggedUser)
    {
        var validator = new UpdateUserValidator();

        var res = await validator.ValidateAsync(request);

        if (loggedUser.Email.Equals(request.Email) == false)
        {
            var userExist = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
            if (userExist)
                res.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesExceptions.VALIDATION_EMAIL_ALREADY_EXISTS));
        }

        if (res.IsValid == false)
            throw new ErrorOnValidationException(res.Errors.Select(error => error.ErrorMessage).ToList());
    }
}