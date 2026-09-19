using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using MyRecipeBook.Application.UseCases.Recipe.GetRecent;
using Shouldly;
using Xunit;

namespace UseCases.Test.Recipe.GetRecents;

public class GetRecentRecipesUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        var recipes = Enumerable
            .Range(0, 6)
            .Select(_ => RecipeBuilder.Build(user))
            .ToList();

        var useCase = CreateUseCase(user, recipes);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Recipes.ShouldNotBeNull();
        result.Recipes.Count.ShouldBe(6);
        result.Recipes.Select(x => x.Id)
            .ShouldBe(recipes.Select(x => x.Id));
    }

    [Fact]
    public async Task Sucess_WhereThereAreNoRecipes()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user, []);

        var result = await useCase.Execute();
        
        result.ShouldNotBeNull();
        result.Recipes.ShouldBeEmpty();
    }
    

    private IGetRecentRecipesUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user,
        IList<MyRecipeBook.Domain.Entities.Recipe> recipe)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var readOnlyRepository = new RecipeReadOnlyRepositoryBuilder();

        readOnlyRepository.GetRecents(user.Id, recipe);

        return new GetRecentRecipesUseCase(loggedUser, readOnlyRepository.Build());
    }
}