using Mapster;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;

namespace MyRecipeBook.Application.UseCases.Recipe.GetRecent;

public class GetRecentRecipesUseCase: IGetRecentRecipesUseCase
{
    private readonly IRecipeReadOnlyRepository _readOnlyRepository;
    private readonly ILoggedUser _loggedUser;

    public GetRecentRecipesUseCase(ILoggedUser loggedUser, IRecipeReadOnlyRepository readOnlyRepository)
    {
        _loggedUser = loggedUser;
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<ResponseRecipesJson> Execute()
    {
        var userId = _loggedUser.GetUserId();

        var recipes = await _readOnlyRepository.GetRecents(userId);

        return new ResponseRecipesJson()
        {
            Recipes = recipes.Adapt<IList<ResponseRecipeSummaryJson>>()
        };
    }
}