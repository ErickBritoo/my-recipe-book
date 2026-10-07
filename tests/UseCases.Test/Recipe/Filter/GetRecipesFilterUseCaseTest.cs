using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Repositories.Recipe;
using Shouldly;
using Xunit;

namespace UseCases.Test.Recipe.Filter;

public class GetRecipesFilterUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        
        var request = new RequestRecipeFilterJson();

        var useCase = CreateUseCase(user, [recipe]);

        var result = await useCase.Execute(request);

        var recipeResponse = result.Recipes.ShouldHaveSingleItem();
        recipeResponse.Id.ShouldBe(recipe.Id);
        recipeResponse.Title.ShouldBe(recipe.Title);
    }
    
    [Fact]
    public async Task Success_WhenRequestIsNull()
    {
        var (user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);

        var useCase = CreateUseCase(user, [recipe]);

        var result = await useCase.Execute(request: null);

        var recipeResponse = result.Recipes.ShouldHaveSingleItem();
        recipeResponse.Id.ShouldBe(recipe.Id);
        recipeResponse.Title.ShouldBe(recipe.Title);
    }

    private GetRecipesFilterUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, IList<MyRecipeBook.Domain.Entities.Recipe> recipe)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var readOnlyRepository = new RecipeReadOnlyRepositoryBuilder();

        readOnlyRepository.Filter(user.Id, recipe);

        return new GetRecipesFilterUseCase(loggedUser, readOnlyRepository.Build());
    }
}