using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.Login.WithEmailAndPassword;

public class LoginWithEmailAndPasswordTests : BaseIntegrationTest
{
    private readonly UserIdentiyManager _user1;
    private const string REQUEST_URI = "authenticate";

    public LoginWithEmailAndPasswordTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }
    
    [Fact]
    public async Task Sucess()
    {
        var request = new RequestLoginJson
        {
            Email = _user1.GetEmail(),
            Password = _user1.GetPassword()
        };
        
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Post(REQUEST_URI, request, cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);
        
        responseData.RootElement.GetProperty("name").GetString().ShouldBe(_user1.GetName());
        responseData.RootElement.GetProperty("tokens").GetProperty("acessToken").GetString().ShouldNotBeNullOrEmpty();
        responseData.RootElement.GetProperty("tokens").GetProperty("refreshToken").GetString().ShouldBeNullOrEmpty();    
    }


    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task ShouldThrowException_WhenUserDontExist(string culture)
    {
        var request = RequestLoginJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Post(REQUEST_URI, request, cancellationToken, culture);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessageException =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_LOGIN_INVALID", new CultureInfo(culture));

        responseData.RootElement.GetProperty("errorMessages").EnumerateArray().ShouldSatisfyAllConditions(errorMessages =>
        {
            errorMessages.Count().ShouldBe(1);
            errorMessages.ShouldContain(errorMessage => errorMessage.GetString() == (expectedMessageException));
        });
    }
}
