using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.DeleteById;

public class DeleteRecipeByIdUseCase : IDeleteRecipeByIdUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeDeleteOnlyRepository _recipeDeleteOnlyRepository;

    public DeleteRecipeByIdUseCase(
        ILoggedUser loggedUser,
        IRecipeDeleteOnlyRepository recipeDeleteOnlyRepository)
    {
        _loggedUser = loggedUser;
        _recipeDeleteOnlyRepository = recipeDeleteOnlyRepository;
    }

    public async Task Execute(Guid recipeId)
    {
        var userId = _loggedUser.GetUserId();
        
        var wasDeleted = await _recipeDeleteOnlyRepository.DeleteById(recipeId, userId);

        if (wasDeleted == false)
            throw new NotFoundException(ResourceMessagesExceptions.VALIDATION_NOT_FOUND_RECIPE);
    }
    
}