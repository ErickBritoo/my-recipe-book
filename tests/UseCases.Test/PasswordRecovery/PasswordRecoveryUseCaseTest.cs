using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.PasswordRecovery;
using MyRecipeBook.Application.UseCases.PasswordRecovery.RequestCode;
using MyRecipeBook.Communication.Requests;
using Shouldly;
using Xunit;

namespace UseCases.Test.PasswordRecovery;

public class PasswordRecoveryUseCaseTest
{

    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();

        var request = new RequestPasswordRecovery() { Email = user.Email };

        var useCase = CreateUseCase(user);

        await useCase.Execute(request).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Sucess_ThereAreNoUser_By_Email()
    {
        var request = RequestPasswordRecoveryJsonBuilder.Build();

        var useCase = CreateUseCase();

        await useCase.Execute(request).ShouldNotThrowAsync();
    }
    
    private PasswordRecoveryUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
    {
        var verificationCodeWriteOnlyRepository = VerificationCodeWriteOnlyRepositoryBuilder.Build();
        var unityOfWork = UnityOfWorkBuilder.Build();
        var userReadOnlyRepository = new UserReadOnlyRepositoryBuilder();

        if (user != null)
            userReadOnlyRepository.GetByEmail(user);

        return new PasswordRecoveryUseCase(userReadOnlyRepository.Build(), verificationCodeWriteOnlyRepository,
            unityOfWork);
    }
}