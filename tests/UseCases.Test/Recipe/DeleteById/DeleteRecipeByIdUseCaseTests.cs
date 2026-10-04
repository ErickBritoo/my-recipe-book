using System.Net;
using System.Reflection;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.DeleteById;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Recipe.DeleteById;

public class DeleteRecipeByIdUseCaseTests
{
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        
        var useCase = CreateUseCase(recipe.Id, user);

        await useCase.Execute(recipe.Id).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Validate_ThrowException_WhenRecipeNotFound()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(recipe.Id, user);

        var exception = await useCase.Execute(Guid.CreateVersion7()).ShouldThrowAsync<NotFoundException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count.ShouldBe(1);
            errorMessages.ShouldHaveSingleItem(ResourceMessagesExceptions.VALIDATION_NOT_FOUND_RECIPE);
        });
    }

    private IDeleteRecipeByIdUseCase CreateUseCase(Guid recipeId, MyRecipeBook.Domain.Entities.User user)
    {
        var deleteOnlyRepository = new RecipeDeleteOnlyRepositoryBuilder().DeleteById(recipeId, user.Id).Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        return new DeleteRecipeByIdUseCase(loggedUser, deleteOnlyRepository);
    }
}