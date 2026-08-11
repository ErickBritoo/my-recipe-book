using Moq;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Security.PasswordHashing;

namespace CommonTestUtilities.Repositories;

public class PasswordHasherBuilder
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    
    public PasswordHasherBuilder(string? password = null)
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();

        _passwordHasherMock.Setup(passwordHasher => passwordHasher.HashPassword(It.IsAny<string>()))
            .Returns("hashed-password");
        
        if (password.IsNotEmpty())
            _passwordHasherMock.Setup(passwordHasher => passwordHasher.VerifyPassword(password, string.Empty)).Returns(true);
    }

    public IPasswordHasher Build() => _passwordHasherMock.Object;
    
}