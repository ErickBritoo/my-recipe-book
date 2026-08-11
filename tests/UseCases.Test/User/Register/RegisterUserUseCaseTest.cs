using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.User.Register;

public class RegisterUserUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase();

        var act = async () => await useCase.Execute(request);

        var result = await act.Invoke();
        
        result.ShouldNotBeNull();
        result.Tokens.ShouldNotBeNull();
        result.Name.ShouldBe(request.Name);
        result.Tokens.AcessToken.ShouldBeNullOrEmpty();
        result.Tokens.RefreshToken.ShouldBeNullOrEmpty();
    }

    [Fact]
    public async Task Validate_ThrowException_WhenNameEmpty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;
        var useCase = CreateUseCase();

        var act = async () => await useCase.Execute(request);

        var exceptionResult = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exceptionResult.ShouldSatisfyAllConditions(exception =>
        {
            exception.ErrorMessages.Count.ShouldBe(1);
            exception.ErrorMessages.ShouldContain(ResourceMessagesExceptions.VALIDATION_NAME_REQUIRED);
        });
    }

    [Fact]
    public async Task Validate_ThrowException_WhenEmailRegistered()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase(request.Email);
        
        var act = async () => await useCase.Execute(request);

        var exceptionResult = await act.ShouldThrowAsync<ErrorOnValidationException>();
        
        exceptionResult.ShouldSatisfyAllConditions(exception =>
        {
            exception.ErrorMessages.Count.ShouldBe(1);
            exception.ErrorMessages.ShouldContain(ResourceMessagesExceptions.VALIDATION_EMAIL_ALREADY_EXISTS);
        });
    }
    
    private static IRegisterUserUseCase CreateUseCase(string? email = null)
    {
        var unityOfWork = UnityOfWorkBuilder.Build();
        var userWriteOnlyRepository = UserWriteOnlyRepositoryBuilder.Build();
        var passwordHasher = new PasswordHasherBuilder().Build();
        var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();

        if (email.IsNotEmpty())
        {
            userReadOnlyRepositoryBuilder.ExistActiveUserWithEmail(email);
        }
        
        return new RegisterUserUseCase(passwordHasher, userWriteOnlyRepository, userReadOnlyRepositoryBuilder.Build(), unityOfWork);
    } 
    
}