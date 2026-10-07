using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyRecipeBook.Domain.DTOs;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infrastructure.DataAcess.Repositories;

internal class RecipeRepository : IRecipeWriteOnlyRepository, IRecipeReadOnlyRepository, IRecipeDeleteOnlyRepository, IRecipeUpdateOnlyRepository
{
    private readonly MyRecipeBookDbContext _dbContext;

    public RecipeRepository(MyRecipeBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Recipe recipe) => await _dbContext.Recipes.AddAsync(recipe);

    async Task<Recipe?> IRecipeReadOnlyRepository.GetById(Guid recipeId, Guid userId)
    {
        return await GetFullRecipe(recipeId, userId)
            .AsNoTracking()
            .FirstOrDefaultAsync(recipe =>
                recipe.Active &&
                recipe.Id == recipeId &&
                recipe.UserId == userId);
    }

    public async Task<IList<Recipe>> GetRecents(Guid userId)
    {
        return await _dbContext.Recipes
            .AsNoTracking()
            .Where(recipe => recipe.UserId == userId)
            .OrderByDescending(recipe => recipe.Id)
            .Take(6)
            .ToListAsync();
    }

    public async Task<IList<RecipeSummaryDto>> Filter(Guid userId, RecipeFilterDto filters)
    {
        var query = _dbContext.Recipes.Where(recipe => recipe.UserId == userId);

        if (filters.Term.IsNotEmpty())
        {
            var term = filters.Term;
            
            query = query.Where(recipe =>
                recipe.Title.Contains(term) ||
                recipe.RecipeIngredients.Any(ingredient =>
                    ingredient.Item.Contains(term)));
        }

        if (filters.CookTime != null)
            query = query.Where(recipe => recipe.CookTime == filters.CookTime);

        if (filters.DishTypes.Any())
        {
            var recipesWithDishTypes = query.Where(recipe => recipe.RecipeDishTypes.Any(dish => dish.Type == filters.DishTypes[0]));
            
            foreach (var dishType in filters.DishTypes.Skip(1))
            {
                // Union -> Retorna se atender a pelo menos uma das condições
                recipesWithDishTypes = recipesWithDishTypes.Union(query.Where(recipe => recipe.RecipeDishTypes.Any(dish => dish.Type == dishType)));            
            }
        
            query = recipesWithDishTypes;
        }
        
        return await query
            .Select(recipe => new RecipeSummaryDto(recipe.Id, recipe.Title))
            .ToListAsync();
    }

    public async Task<bool> DeleteById(Guid recipeId, Guid userId)
    {
        var rows = await _dbContext.Recipes
            .Where(recipe => recipe.Active && recipe.Id == recipeId && recipe.UserId == userId)
            .ExecuteDeleteAsync();

        return rows > 0;
    }

    async Task<Recipe?> IRecipeUpdateOnlyRepository.GetById(Guid recipeId, Guid userId)
    {
        return await GetFullRecipe(recipeId, userId).FirstOrDefaultAsync(recipe =>
            recipe.Active &&
            recipe.Id == recipeId &&
            recipe.UserId == userId);
    }

    private IIncludableQueryable<Recipe, ICollection<RecipeInstruction>> GetFullRecipe(Guid recipeId, Guid userId)
    {
        return _dbContext.Recipes
            .Include(recipe => recipe.RecipeDishTypes)
            .Include(recipe => recipe.RecipeIngredients)
            .Include(recipe => recipe.RecipeInstructions);
    }
}