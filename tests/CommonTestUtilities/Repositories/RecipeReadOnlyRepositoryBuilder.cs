using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;

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
}