using Moq;
using MyRecipeBook.Domain.Repositories;

namespace CommonTestUtilities.Repositories;

public static class UnityOfWorkBuilder
{
    public static IUnityOfWork Build()
    {
        return new Mock<IUnityOfWork>().Object;
    }
}