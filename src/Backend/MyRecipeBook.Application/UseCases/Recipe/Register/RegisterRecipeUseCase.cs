using System.Linq;
using System.Threading.Tasks;
using Mapster;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Register;

public class RegisterRecipeUseCase : IRegisterRecipeUseCase
{
    private readonly IRecipeWriteOnlyRepository _recipeWriteOnlyRepository;
    private readonly IUnityOfWork _unityOfWork;
    private readonly ILoggedUser _loggedUser;
    
    public RegisterRecipeUseCase(
        IRecipeWriteOnlyRepository recipeWriteOnlyRepository, 
        IUnityOfWork unityOfWork, 
        ILoggedUser loggedUser)
    {
        _recipeWriteOnlyRepository = recipeWriteOnlyRepository;
        _unityOfWork = unityOfWork;
        _loggedUser = loggedUser;
    }
    
    
    public async Task<ResponseRegisteredRecipeJson> Execute(RequestRecipeJson request)
    {
        await Validate(request);

        var recipe = request.Adapt<Domain.Entities.Recipe>();
        recipe.UserId = _loggedUser.GetUserId();
        
        await _recipeWriteOnlyRepository.Add(recipe);

        await _unityOfWork.Commit();

        return new ResponseRegisteredRecipeJson()
        {
            Id = recipe.Id,
            Title = recipe.Title
        };
    }

    private static async Task Validate(RequestRecipeJson request)
    {
        var validator = new RecipeValidator();

        var res = await validator.ValidateAsync(request);

        if (res.IsValid == false)
        {
            throw new ErrorOnValidationException(res.Errors.Select(e => e.ErrorMessage).ToList());
        }
    }
}