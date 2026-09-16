using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.Recipe.Update;

public interface IUpdateRecipeUseCase
{
    Task Execute(Guid recipeId, RequestRecipeJson request);
}