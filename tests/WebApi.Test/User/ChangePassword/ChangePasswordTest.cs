using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.User.ChangePassword;

public class ChangePasswordTest : BaseIntegrationTest
{
    private readonly string REQUEST_URI = "users/password";
    private readonly UserIdentiyManager _user1;

    public ChangePasswordTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        request.CurrentPassword = _user1.GetPassword();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Put(REQUEST_URI, request, _user1.GetAcessToken(), cancellationToken);
        
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldHaveError_WhenCurrentPasswordDoesNotMatch(string culture)
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;
        
        var response = await Put(REQUEST_URI, request, _user1.GetAcessToken(), cancellationToken, culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);
        
        var expectedMessageException = ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_CURRENT_PASSWORD", new CultureInfo(culture));
        
        responseData.RootElement.GetProperty("errorMessages").EnumerateArray().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(errorMessage => errorMessage.GetString() == expectedMessageException);
        });
    }
}