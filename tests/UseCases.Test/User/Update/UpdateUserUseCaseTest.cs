using System.Net;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.User.Update;

public class UpdateUserUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        var useCase = CreateUseCase(user);

        await useCase.Execute(request).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Validate_ThrowException_WhenNameEmpty()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;
        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count.ShouldBe(1);
            errorsMessages.ShouldContain(ResourceMessagesExceptions.VALIDATION_NAME_REQUIRED);
        });    
    }
    
    [Fact]
    public async Task Validate_ThrowException_WhenEmailAlReadyExists()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        var useCase = CreateUseCase(user, request.Email);

        var act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count.ShouldBe(1);
            errorsMessages.ShouldContain(ResourceMessagesExceptions.VALIDATION_EMAIL_ALREADY_EXISTS);
        });    
    }
    
    
    private UpdateUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? emailThatAlreadyExists = null)
    {
        var unityOfWork = UnityOfWorkBuilder.Build();
        var userUpdateOnlyRepository = UserUpdateOnlyRepositoryBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
        
        if (emailThatAlreadyExists != null)
            userReadOnlyRepositoryBuilder.ExistActiveUserWithEmail(emailThatAlreadyExists);
        
        return new UpdateUserUseCase(loggedUser, userReadOnlyRepositoryBuilder.Build(), userUpdateOnlyRepository, unityOfWork);
    }

}