using System.Net;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.Mappings;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.Recipe.GetById;

public class GetByIdRecipeUseCaseTest
{

    static GetByIdRecipeUseCaseTest()
    {
        MapsterConfig.Configure();
    }
    
    
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(recipe, user);

        var result = await useCase.Execute(recipe.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(recipe.Id);
        result.Title.ShouldBe(recipe.Title);
        
        result.Instructions.Select(i => i.Order).ShouldBeInOrder(SortDirection.Ascending);
    }

    [Fact]
    public async Task Validate_ThrowException_WhenRecipeNull()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(recipe, user);

        var act = async () => await useCase.Execute(Guid.CreateVersion7());
        
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.NotFound);
        exception.GetErrorMessages().ShouldHaveSingleItem(ResourceMessagesExceptions.VALIDATION_NOT_FOUND_RECIPE);

    }
    
    
    private IGetByIdRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.Recipe? recipe, MyRecipeBook.Domain.Entities.User user)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var recipeReadOnlyRepository = new RecipeReadOnlyRepositoryBuilder();

        if (recipe != null)
            recipeReadOnlyRepository.GetById(recipe);
        
        return new GetByIdRecipeUseCase(loggedUser, recipeReadOnlyRepository.Build());
    }
}