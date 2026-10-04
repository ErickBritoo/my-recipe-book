using Moq;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace CommonTestUtilities.Repositories;

public class RecipeDeleteOnlyRepositoryBuilder
{
    private Mock<IRecipeDeleteOnlyRepository> _mock;

    public RecipeDeleteOnlyRepositoryBuilder()
    {
        _mock = new Mock<IRecipeDeleteOnlyRepository>();
    }

    public IRecipeDeleteOnlyRepository Build() => _mock.Object;

    public RecipeDeleteOnlyRepositoryBuilder DeleteById(Guid recipeId, Guid userId)
    {
        _mock.Setup(repository => repository.DeleteById(recipeId, userId)).ReturnsAsync(true);

        return this;
    }
}