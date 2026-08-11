using Moq;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories;

public class UserReadOnlyRepositoryBuilder
{
    private Mock<IUserReadOnlyRepository> _userReadOnlyRepositoryMock;

    public UserReadOnlyRepositoryBuilder()
    {
        _userReadOnlyRepositoryMock = new Mock<IUserReadOnlyRepository>();
    }

    public IUserReadOnlyRepository Build() => _userReadOnlyRepositoryMock.Object;

    public void ExistActiveUserWithEmail(string email)
    {
        _userReadOnlyRepositoryMock.Setup(repository => repository.ExistActiveUserWithEmail(email)).ReturnsAsync(true);
    }
}