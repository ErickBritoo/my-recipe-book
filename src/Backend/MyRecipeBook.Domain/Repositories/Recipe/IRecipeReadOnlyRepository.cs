using MyRecipeBook.Domain.DTOs;

namespace MyRecipeBook.Domain.Repositories.Recipe;

public interface IRecipeReadOnlyRepository
{
    Task<Entities.Recipe?> GetById(Guid recipeId, Guid userId);
    Task<IList<Entities.Recipe>> GetRecents(Guid userId);
    Task<IList<RecipeSummaryDto>> Filter(Guid userId, RecipeFilterDto filters);
}