using Mapster;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Application.Mappings;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<RequestRecipeJson, Recipe>.NewConfig()
            .Map(destination => destination.CookTime,
                request => (MyRecipeBook.Communication.Enums.CookTime)request.CookTime)
            .Map(destination => destination.RecipeDishTypes, request => request.DishTypes.Select(dishType =>
                new RecipeDishType()
                {
                    Type = (DishType)dishType
                }))
            .Map(destination => destination.RecipeIngredients, request => request.Ingredients.Select(ingredient =>
                new RecipeIngredient()
                {
                    Item = ingredient.Item
                }))
            .Map(destination => destination.RecipeInstructions, request => request.Instructions.Select(instruction =>
                new RecipeInstruction()
                {
                    Order = instruction.Order,
                    Description = instruction.Description
                }));

        TypeAdapterConfig<RequestRegisterUserJson, User>.NewConfig()
            .Ignore(destination => destination.Password);
        
        TypeAdapterConfig<Recipe, ResponseRecipeJson>.NewConfig()
            .Map(destination => destination.Instructions, entity =>
                entity.RecipeInstructions.Select(instruction => new ResponseInstructionJson()
                {
                    Description = instruction.Description,
                    Order = instruction.Order
                })
            )
            .Map(destination => destination.Ingredients, entity =>
                entity.RecipeIngredients.Select(ingredients => new ResponseIngredientJson()
                {
                    Item = ingredients.Item
                }))
            .Map(destination => destination.DishTypes, entity =>
                entity.RecipeDishTypes.Select(dishType => (MyRecipeBook.Communication.Enums.DishType)dishType.Type));
    }
}