using Mapster;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;

public class GetByIdRecipeUseCase : IGetByIdRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _recipeReadOnlyRepository;

    public GetByIdRecipeUseCase(ILoggedUser loggedUser, IRecipeReadOnlyRepository recipeReadOnlyRepository)
    {
        _loggedUser = loggedUser;
        _recipeReadOnlyRepository = recipeReadOnlyRepository;
    }
    
    public async Task<ResponseRecipeJson> Execute(Guid recipeId)
    {
        var recipe = await _recipeReadOnlyRepository.GetById(recipeId, _loggedUser.GetUserId());
        
        if (recipe == null)
            throw new NotFoundException(ResourceMessagesExceptions.VALIDATION_NOT_FOUND_RECIPE);

        recipe.RecipeInstructions = recipe.RecipeInstructions.OrderBy(r => r.Order).ToList();
        
        return recipe.Adapt<ResponseRecipeJson>();
    }
}