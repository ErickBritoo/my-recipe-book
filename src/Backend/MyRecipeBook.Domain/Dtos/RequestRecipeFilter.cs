using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Domain.DTOs;

public record RecipeFilterDto
{
    public string? Term { get; init; }
    public CookTime? CookTime { get; init; }
    public IList<DishType> DishTypes { get; init; } = [];
}