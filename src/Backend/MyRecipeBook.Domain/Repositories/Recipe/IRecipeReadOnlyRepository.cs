namespace MyRecipeBook.Domain.Repositories;

public interface IRecipeReadOnlyRepository
{
    Task<Entities.Recipe?> GetById(Guid recipeId, Guid userId);
    Task<IList<Entities.Recipe>> GetRecents(Guid userId);
}