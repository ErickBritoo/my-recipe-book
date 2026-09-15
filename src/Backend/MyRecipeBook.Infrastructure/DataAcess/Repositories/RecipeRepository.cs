using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infrastructure.DataAcess.Repositories;

internal class RecipeRepository : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository, IRecipeDeleteOnlyRepository
{
    private readonly MyRecipeBookDbContext _dbContext;

    public RecipeRepository(MyRecipeBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Recipe recipe) => await _dbContext.Recipes.AddAsync(recipe);

    public async Task<Recipe?> GetById(Guid recipeId, Guid userId) => await _dbContext.Recipes
        .Include(recipe => recipe.RecipeDishTypes)
        .Include(recipe => recipe.RecipeIngredients)
        .Include(recipe => recipe.RecipeInstructions)
        .AsNoTracking()
        .FirstOrDefaultAsync(recipe =>
            recipe.Active &&
            recipe.Id == recipeId &&
            recipe.UserId == userId);

    public async Task<bool> DeleteById(Guid recipeId, Guid userId)
    {
        var rows = await _dbContext.Recipes
            .Where(recipe => recipe.Active && recipe.Id == recipeId && recipe.UserId == userId)
            .ExecuteDeleteAsync();

        return rows > 0;
    }
}