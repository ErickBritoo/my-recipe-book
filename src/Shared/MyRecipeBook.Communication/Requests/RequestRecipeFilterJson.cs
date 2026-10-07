using MyRecipeBook.Communication.Enums;

namespace MyRecipeBook.Communication.Requests;

public class RequestRecipeFilterJson
{
    public string? Term { get; set; }
    public CookTime? CookTime { get; set; }
    public IList<DishType>? DishTypes { get; set; } = [];
}