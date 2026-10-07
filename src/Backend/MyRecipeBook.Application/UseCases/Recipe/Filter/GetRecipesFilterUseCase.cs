using Mapster;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.DTOs;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public class GetRecipesFilterUseCase : IGetRecipesFilterUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeReadOnlyRepository _onlyRepository;

    public GetRecipesFilterUseCase(ILoggedUser loggedUser, IRecipeReadOnlyRepository onlyRepository)
    {
        _loggedUser = loggedUser;
        _onlyRepository = onlyRepository;
    }

    public async Task<ResponseRecipesJson> Execute(RequestRecipeFilterJson request)
    {
        var recipes = await _onlyRepository.Filter(_loggedUser.GetUserId(), request.Adapt<RecipeFilterDto>());

        return new ResponseRecipesJson()
        {
            Recipes = recipes.Adapt<IList<ResponseRecipeSummaryJson>>()
        };
    }
}