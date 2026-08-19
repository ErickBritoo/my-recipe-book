using Moq;
using MyRecipeBook.Domain.Security.PasswordHashing;

namespace CommonTestUtilities.Security;

public class PasswordHasherBuilder
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public PasswordHasherBuilder()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _passwordHasherMock.Setup(passwordHasher => passwordHasher.HashPassword(It.IsAny<string>()))
            .Returns("hashed-password");
    }

    public IPasswordHasher Build() => _passwordHasherMock.Object;

    public void VerifyPassword(string password)
    {
        _passwordHasherMock.Setup(passwordHasher => passwordHasher.VerifyPassword(password, It.IsAny<string>()))
            .Returns(true);
    }
}