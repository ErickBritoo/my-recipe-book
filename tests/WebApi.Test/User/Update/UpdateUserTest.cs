using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.User.Update;

public class UpdateUserTest : BaseIntegrationTest
{
    private readonly string REQUEST_URI = "users/profile";
    private UserIdentiyManager _user1;

    public UpdateUserTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Put(REQUEST_URI, request, _user1.GetAcessToken(), cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var userWasUpdatedInDatabase = dbContext.Users.Any(user =>
            user.Active
            && user.Email.Equals(request.Email)
            && user.Name.Equals(request.Name));

        userWasUpdatedInDatabase.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldHaveError_WhenNameIsEmpty(string culture)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Put(REQUEST_URI, request, _user1.GetAcessToken(), cancellationToken, culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessageException =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_NAME_REQUIRED", new CultureInfo(culture));

        responseData.RootElement.GetProperty("errorMessages").EnumerateArray().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count().ShouldBe(1);
            errorsMessages.ShouldContain(errorMessage => errorMessage.GetString() == expectedMessageException);
        });
    }
}