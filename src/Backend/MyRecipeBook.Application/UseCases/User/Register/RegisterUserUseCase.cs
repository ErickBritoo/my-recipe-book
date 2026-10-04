using FluentValidation.Results;
using Mapster;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Register;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUnityOfWork _unityOfWork;
    private readonly IAcessTokenGenerator _acessTokenGenerator;
    
    public RegisterUserUseCase(
        IPasswordHasher passwordHasher,
        IUserWriteOnlyRepository userWriteOnlyRepository,
        IUserReadOnlyRepository userReadOnlyRepository,
        IUnityOfWork unityOfWork,
        IAcessTokenGenerator acessTokenGenerator 
    )
    {
        _passwordHasher = passwordHasher;
        _userWriteOnlyRepository = userWriteOnlyRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _unityOfWork = unityOfWork;
        _acessTokenGenerator = acessTokenGenerator;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {
        await ValidateAndThrowOnFailures(request);

        var user = request.Adapt<Domain.Entities.User>();
        user.Password = _passwordHasher.HashPassword(request.Password);

        await _userWriteOnlyRepository.Add(user);

        await _unityOfWork.Commit();

        return new ResponseRegisteredUserJson()
        {
            Name = user.Name,
            Tokens = new ResponseTokenJson()
            {
                AcessToken = _acessTokenGenerator.Generate(user)
            }
        };
    }

    private async Task ValidateAndThrowOnFailures(RequestRegisterUserJson request)
    {
        var validator = new RegisterUserValidator();

        var res = await validator.ValidateAsync(request);

        var existUserActiveWithEmail = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);

        if (existUserActiveWithEmail)
        {
            res.Errors.Add(new ValidationFailure(string.Empty,
                ResourceMessagesExceptions.VALIDATION_EMAIL_ALREADY_EXISTS));
        }

        if (res.IsValid == false)
        {
            throw new ErrorOnValidationException(res.Errors.Select(e => e.ErrorMessage).ToList());
        }
    }
}