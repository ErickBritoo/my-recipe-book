using System.Net;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Recipe.Register;

public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Title.ShouldBe(request.Title);
    }

    [Fact]
    public async Task Validate_ThrowException_WhenTitleEmpty()
    {
        var (user, _) = UserBuilder.Build();
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var useCase = CreateUseCase(user);

        var act = async () => await useCase.Execute(request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count.ShouldBe(1);
            errorsMessages.ShouldContain(errorMessage => errorMessage.Equals(ResourceMessagesExceptions.VALIDATION_TITLE_REQUIRED));
        });
    }

    private IRegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var unityOfWork = UnityOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var readOnlyRepository = RecipeWriteOnlyRepositoryBuilder.Build();

        return new RegisterRecipeUseCase(readOnlyRepository, unityOfWork, loggedUser);
    }
}