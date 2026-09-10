using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Domain.Entities;

public class Recipe : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = [];
    public ICollection<RecipeDishType> RecipeDishTypes { get; set; } = [];
    public ICollection<RecipeInstruction> RecipeInstructions { get; set; } = [];
    public CookTime CookTime { get; set; }
    public Guid UserId { get; set; }
}