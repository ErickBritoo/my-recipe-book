using System.Globalization;
using System.Net;
using System.Text.Json;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.Recipe.GetRecents;

public class GetRecentsRecipesInvalidTokenTest : BaseIntegrationTest
{
    private const string REQUEST_URI = "/recipes/recent";
    private readonly string ACESS_TOKEN_NOT_EXISTS_IN_DATABASE;

    public GetRecentsRecipesInvalidTokenTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        ACESS_TOKEN_NOT_EXISTS_IN_DATABASE = factory.Token_User_Not_Found_In_Database;
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenAcessTokenIsEmpty()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Get(REQUEST_URI, cancellationToken, acessToken: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessage =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_ACESS_TOKEN_REQUIRED",
                new CultureInfo("en"));

        responseData.RootElement.GetProperty("errorMessages").EnumerateArray()
            .ShouldSatisfyAllConditions(errorsMessages =>
            {
                errorsMessages.Count().ShouldBe(1);
                errorsMessages.ShouldAllBe(errorMessage => errorMessage.GetString() == expectedMessage);
            });
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenAcessTokenIsInvalid()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Get(REQUEST_URI, cancellationToken, acessToken: "invalidToken");
        
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessage =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_RESOURCE_ACESS_DENIED",
                new CultureInfo("en"));

        responseData.RootElement.GetProperty("errorMessages").EnumerateArray()
            .ShouldSatisfyAllConditions(errorsMessages =>
            {
                errorsMessages.Count().ShouldBe(1);
                errorsMessages.ShouldAllBe(errorMessage => errorMessage.GetString() == expectedMessage);
            });
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenUserFromAcessTokenDoesNotExist()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Get(REQUEST_URI, cancellationToken, acessToken: ACESS_TOKEN_NOT_EXISTS_IN_DATABASE);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessage =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_RESOURCE_ACESS_DENIED",
                new CultureInfo("en"));

        responseData.RootElement.GetProperty("errorMessages").EnumerateArray()
            .ShouldSatisfyAllConditions(errorsMessages =>
            {
                errorsMessages.Count().ShouldBe(1);
                errorsMessages.ShouldAllBe(errorMessage => errorMessage.GetString() == expectedMessage);
            });
    }
}