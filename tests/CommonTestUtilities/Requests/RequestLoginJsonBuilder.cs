using Bogus;
using CommonTestUtilities.Repositories;
using Moq;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Security.PasswordHashing;

namespace CommonTestUtilities.Requests;

public class RequestLoginJsonBuilder
{
    public static RequestLoginJson Build()
    {
        return new Faker<RequestLoginJson>()
            .RuleFor(request => request.Email, f => f.Internet.Email())
            .RuleFor(request => request.Password, f => f.Internet.Password());
    }
}