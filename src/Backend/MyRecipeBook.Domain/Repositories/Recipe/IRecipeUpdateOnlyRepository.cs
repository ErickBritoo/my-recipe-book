namespace MyRecipeBook.Domain.Repositories;

public interface IRecipeUpdateOnlyRepository
{
    Task<Entities.Recipe?> GetById(Guid recipeId, Guid userId);
}