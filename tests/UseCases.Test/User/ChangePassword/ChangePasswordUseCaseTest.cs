using System.Net;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.User.ChangePassword;

public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();
        var useCase = CreateUseCase(user, request.CurrentPassword);

       await useCase.Execute(request).ShouldNotThrowAsync();
    }
    
    [Fact]
    public async Task Validate_ThrowException_WhenNewPasswordEmpty()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = string.Empty;
        var useCase = CreateUseCase(user, request.CurrentPassword);

        var act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count.ShouldBe(1);
            errorsMessages.ShouldContain(ResourceMessagesExceptions.VALIDATION_PASSWORD_REQUIRED);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenCurrentPasswordDoesNotMatch()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();
        var useCase = CreateUseCase(user, "invalidPassword");    
        
        var act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count.ShouldBe(1);
            errorsMessages.ShouldContain(ResourceMessagesExceptions.VALIDATION_CURRENT_PASSWORD);
        });
    }
    
    private IChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string password)
    {
        var passwordHasher = new PasswordHasherBuilder().VerifyPassword(password).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var userUpdateOnlyRepository = UserUpdateOnlyRepositoryBuilder.Build();
        
        return new ChangePasswordUseCase(passwordHasher, loggedUser, userUpdateOnlyRepository);
    }
}