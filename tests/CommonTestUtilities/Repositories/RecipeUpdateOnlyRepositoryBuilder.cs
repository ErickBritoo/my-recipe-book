using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;

namespace CommonTestUtilities.Repositories;

public class RecipeUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IRecipeUpdateOnlyRepository> _mock;
    
    public RecipeUpdateOnlyRepositoryBuilder()
    {
        _mock = new Mock<IRecipeUpdateOnlyRepository>();
        
    }

    public IRecipeUpdateOnlyRepository Build() => _mock.Object;

    public RecipeUpdateOnlyRepositoryBuilder GetById(Recipe recipe)
    {
        _mock.Setup(repository => repository.GetById(recipe.Id, recipe.UserId)).ReturnsAsync(recipe);

        return this;
    }
}