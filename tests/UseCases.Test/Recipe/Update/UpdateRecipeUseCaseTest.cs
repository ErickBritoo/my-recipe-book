using System.Net;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Recipe.Update;

public class UpdateRecipeUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
            
        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(recipe, user);

        await useCase.Execute(recipe.Id, request).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Validate_ThrowException_WhenTitleEmpty()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
            
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;
        
        var useCase = CreateUseCase(recipe, user);

        var act = async () => await useCase.Execute(recipe.Id, request);

        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count.ShouldBe(1);
            errorsMessages.ShouldContain(errorMessage => errorMessage.Equals(ResourceMessagesExceptions.VALIDATION_TITLE_REQUIRED));
        });
    }

    [Fact]
    public async Task Validate_ThrowException_WhenRecipeNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
            
        var request = RequestRecipeJsonBuilder.Build();
        
        var useCase = CreateUseCase(recipe, user);

        var act = async () => await useCase.Execute(Guid.CreateVersion7(), request);

        var exception = await act.ShouldThrowAsync<NotFoundException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count.ShouldBe(1);
            errorsMessages.ShouldContain(errorMessage => errorMessage.Equals(ResourceMessagesExceptions.VALIDATION_NOT_FOUND_RECIPE));
        });
    }
    
    private UpdateRecipeUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.Recipe recipe,
        MyRecipeBook.Domain.Entities.User user)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var unityOfWork = UnityOfWorkBuilder.Build();
        var updateOnlyRepository = new RecipeUpdateOnlyRepositoryBuilder().GetById(recipe).Build();
        
        return new UpdateRecipeUseCase(loggedUser, updateOnlyRepository, unityOfWork);
    }
}