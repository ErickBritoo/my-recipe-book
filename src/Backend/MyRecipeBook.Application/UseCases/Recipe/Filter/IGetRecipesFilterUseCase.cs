using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.Recipe.Filter;

public interface IGetRecipesFilterUseCase
{
    Task<ResponseRecipesJson> Execute(RequestRecipeFilterJson request);
}