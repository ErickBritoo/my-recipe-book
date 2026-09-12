using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Application.UseCases.Recipe.GetById;

public interface IGetByIdRecipeUseCase
{
    public Task<ResponseRecipeJson> Execute(Guid recipeId);
}

