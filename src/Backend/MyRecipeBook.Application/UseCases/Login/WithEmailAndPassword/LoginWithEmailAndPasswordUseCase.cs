using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordUseCase : ILoginWithEmailAndPasswordUseCase
{
    private IUserReadOnlyRepository _readOnlyRepository;
    private IPasswordHasher _passwordHasher;
    private IAcessTokenGenerator _acessTokenGenerator;
    
    public LoginWithEmailAndPasswordUseCase(
        IUserReadOnlyRepository readOnlyRepository, 
        IPasswordHasher passwordHasher,
        IAcessTokenGenerator acessTokenGenerator)
    {
        _readOnlyRepository = readOnlyRepository;
        _passwordHasher = passwordHasher;
        _acessTokenGenerator = acessTokenGenerator;
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
            Tokens = new ResponseTokenJson()
            {
                AcessToken = _acessTokenGenerator.Generate(user)
            }
        };
    }
}