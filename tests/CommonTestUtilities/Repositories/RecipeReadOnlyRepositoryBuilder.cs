using Mapster;
using Moq;
using MyRecipeBook.Domain.DTOs;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestUtilities.Repositories;

public class RecipeReadOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeReadOnlyRepository> _mock;

    public RecipeReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IRecipeReadOnlyRepository>();
    }

    public IRecipeReadOnlyRepository Build() => _mock.Object;

    public void GetById(Recipe recipe) =>
        _mock.Setup(repository => repository.GetById(recipe.Id, recipe.UserId)).ReturnsAsync(recipe);

    public void GetRecents(Guid userId, IList<Recipe> recipes) =>
        _mock.Setup(repository => repository.GetRecents(userId)).ReturnsAsync(recipes);

    public void Filter(Guid userId, IList<Recipe> recipes)
    {
        _mock.Setup(repository => repository.Filter(userId, It.IsAny<RecipeFilterDto>()))
            .ReturnsAsync(recipes.Adapt<IList<RecipeSummaryDto>>());
    }
}