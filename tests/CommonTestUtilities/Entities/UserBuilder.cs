using Bogus;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Security;
using MyRecipeBook.Domain.Entities;

namespace CommonTestUtilities.Entities;

public class UserBuilder
{
    public static (User user, string password) Build()
    {
        var password = GenerateRandomPassword();

        var user = new Faker<User>()
            .RuleFor(user => user.Name, f => f.Person.FirstName)
            .RuleFor(user => user.Email, (f, user) => f.Internet.Email(user.Name))
            .RuleFor(user => user.Password, f => password);

        return (user, password);
    }

    private static string GenerateRandomPassword()
    {
        var passwordEncripter = new PasswordHasherBuilder().Build();

        var password = new Faker().Internet.Password();

        return passwordEncripter.HashPassword(password);
    }
}