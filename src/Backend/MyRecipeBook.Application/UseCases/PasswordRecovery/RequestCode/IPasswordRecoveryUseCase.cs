using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.RequestCode;

public interface IPasswordRecoveryUseCase
{
    Task Execute(RequestPasswordRecovery request);
}