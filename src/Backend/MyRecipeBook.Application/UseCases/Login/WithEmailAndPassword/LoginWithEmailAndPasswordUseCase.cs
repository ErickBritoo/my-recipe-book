using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordUseCase : ILoginWithEmailAndPasswordUseCase
{
    private IUserReadOnlyRepository _readOnlyRepository;
    private IPasswordHasher _passwordHasher;
    
    public LoginWithEmailAndPasswordUseCase(
        IUserReadOnlyRepository readOnlyRepository, 
        IPasswordHasher passwordHasher)
    {
        _readOnlyRepository = readOnlyRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestLoginJson request)
    {
        var user = await _readOnlyRepository.GetByEmail(request.Email);

        if (user is null)
            throw new InvalidLoginException();

        var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.Password);

        if (isPasswordValid == false)
            throw new InvalidLoginException();

        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
        };
    }
}