using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;

public interface IPasswordResetUseCase
{
    Task Execute(RequestResetRecoveryPasswordJson request);
}