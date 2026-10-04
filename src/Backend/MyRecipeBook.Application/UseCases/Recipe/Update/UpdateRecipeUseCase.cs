using Mapster;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Update;

public class UpdateRecipeUseCase: IUpdateRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeUpdateOnlyRepository _updateOnlyRepository;
    private readonly IUnityOfWork _unityOfWork;
    
    
    public UpdateRecipeUseCase(
        ILoggedUser loggedUser, 
        IRecipeUpdateOnlyRepository updateOnlyRepository,
        IUnityOfWork unityOfWork)
    {
        _loggedUser = loggedUser;
        _updateOnlyRepository = updateOnlyRepository;
        _unityOfWork = unityOfWork;
    }
    
    
    public async Task Execute(Guid recipeId, RequestRecipeJson request)
    {
        await Validate(request);
        
        var recipe = await _updateOnlyRepository.GetById(recipeId, _loggedUser.GetUserId());

        if (recipe == null)
            throw new NotFoundException(ResourceMessagesExceptions.VALIDATION_NOT_FOUND_RECIPE);

        request.Adapt(recipe);

        await _unityOfWork.Commit();
    }


    private async Task Validate(RequestRecipeJson request)
    {
        var validator = new RecipeValidator();
        
        var res = await validator.ValidateAsync(request);

        if (res.IsValid == false)
            throw new ErrorOnValidationException(res.Errors.Select(error => error.ErrorMessage).ToList());
    }
}