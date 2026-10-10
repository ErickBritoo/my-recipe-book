using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.VerificationCode;

namespace CommonTestUtilities.Repositories;

public class IVerificationCodeReadOnlyRepositoryBuilder
{
    private Mock<IVerificationCodeReadOnlyRepository> _mock;

    public IVerificationCodeReadOnlyRepositoryBuilder()
    {
        _mock = new Mock<IVerificationCodeReadOnlyRepository>();
    }

    public IVerificationCodeReadOnlyRepository Build() => _mock.Object;

    public void Get(VerificationCode verificationCode)
    {
        _mock.Setup(repository => repository.Get(verificationCode.UserId, verificationCode.Code, verificationCode.Type))
            .ReturnsAsync(verificationCode);
    }
}