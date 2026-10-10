using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public static class RequestPasswordRecoveryJsonBuilder
{
    public static RequestPasswordRecovery Build()
    {
        return new Faker<RequestPasswordRecovery>()
            .RuleFor(request => request.Email, f => f.Internet.Email());
    }
}