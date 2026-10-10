using Moq;
using MyRecipeBook.Domain.Repositories.VerificationCode;

namespace CommonTestUtilities.Repositories;

public class VerificationCodeWriteOnlyRepositoryBuilder
{
    public static IVerificationCodeWriteOnlyRepository Build()
    {
        return new Mock<IVerificationCodeWriteOnlyRepository>().Object;
    }
}