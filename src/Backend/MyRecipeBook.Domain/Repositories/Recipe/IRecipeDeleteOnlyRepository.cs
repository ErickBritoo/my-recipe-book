namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeDeleteOnlyRepository
{
    Task<bool> DeleteById(Guid recipeId, Guid userId);
}