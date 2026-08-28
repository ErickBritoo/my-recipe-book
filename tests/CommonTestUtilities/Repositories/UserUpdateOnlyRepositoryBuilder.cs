using Moq;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories;

public static class UserUpdateOnlyRepositoryBuilder
{
    public static IUserUpdateOnlyRepository Build() => new Mock<IUserUpdateOnlyRepository>().Object;
}