using Bogus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;

namespace CommonTestUtilities.Entities;

public static class RecipeBuilder
{
    public static Recipe Build(User user)
    {
        var instructionOrder = 5;

        var faker = new Faker<Recipe>()
            .RuleFor(recipe => recipe.UserId, user.Id)
            .RuleFor(recipe => recipe.Title, f => f.Lorem.Word())
            .RuleFor(recipe => recipe.CookTime, f => f.PickRandom<CookTime>())
            .RuleFor(recipe => recipe.RecipeDishTypes, f => f.Make(3, () => new RecipeDishType()
            {
                Type = f.PickRandom<DishType>()
            }))
            .RuleFor(recipe => recipe.RecipeInstructions, f => f.Make(5, () => new RecipeInstruction()
            {
                Order = instructionOrder--,
                Description = f.Lorem.Sentence()
            }))
            .RuleFor(recipe => recipe.RecipeIngredients, f => f.Make(8, () => new RecipeIngredient()
            {
                Item = f.Lorem.Word()
            }));


        return faker;
    }
}