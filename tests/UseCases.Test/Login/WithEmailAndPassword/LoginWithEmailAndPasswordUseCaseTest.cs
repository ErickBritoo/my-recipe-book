using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var (user, password) = UserBuilder.Build();
        var request = new RequestLoginJson
        {
            Email = user.Email,
            Password = password
        };

        var useCase = CreateUseCase(request.Password, user);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Tokens.ShouldNotBeNull();
        result.Name.ShouldBe(user.Name);
        result.Tokens.AcessToken.ShouldNotBeNullOrEmpty();
        result.Tokens.RefreshToken.ShouldBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_ThrowException_WhenUserDontExist()
    {
        var request = RequestLoginJsonBuilder.Build();
        var useCase = CreateUseCase();

        var act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<InvalidLoginException>();

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldContain(ResourceMessagesExceptions.VALIDATION_LOGIN_INVALID);
        });
    }

    [Fact]
    public async Task Should_ThrowException_WhenPasswordIsIncorrect()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestLoginJsonBuilder.Build();
        request.Email = user.Email;

        var useCase = CreateUseCase(user: user);

        var act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<InvalidLoginException>();

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldContain(ResourceMessagesExceptions.VALIDATION_LOGIN_INVALID);
        });
    }

    private static ILoginWithEmailAndPasswordUseCase CreateUseCase(string? password = null,
        MyRecipeBook.Domain.Entities.User? user = null)
    {
        var readOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
        var passwordHasherBuilder = new PasswordHasherBuilder();
        var acessTokenGenerator = AcessTokenGeneratorBuilder.Build();

        if (user is not null)
            readOnlyRepositoryBuilder.GetByEmail(user);

        if (password is not null)
            passwordHasherBuilder.VerifyPassword(password);

        return new LoginWithEmailAndPasswordUseCase(readOnlyRepositoryBuilder.Build(), passwordHasherBuilder.Build(), acessTokenGenerator);
    }
}