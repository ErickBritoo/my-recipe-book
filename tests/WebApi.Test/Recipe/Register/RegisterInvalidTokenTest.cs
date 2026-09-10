using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using Xunit;

namespace WebApi.Test.Recipe.Register;

public class RegisterInvalidTokenTest : BaseIntegrationTest
{
    private readonly string REQUEST_URI = "/recipes";
    private readonly string ACESS_TOKEN_NOT_EXISTS_IN_DATABASE;
    
    public RegisterInvalidTokenTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        ACESS_TOKEN_NOT_EXISTS_IN_DATABASE = factory.Token_User_Not_Found_In_Database;
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenAccessTokenIsInvalid()
    {
        var request = RequestRecipeJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Post(REQUEST_URI, request, cancellationToken, acessToken: "invalidToken");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var respondeBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(respondeBody, cancellationToken: cancellationToken);
        
        var expectedMessage =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_RESOURCE_ACESS_DENIED",
                new CultureInfo("en"));
        
        responseData.RootElement.GetProperty("errorMessages").EnumerateArray()
            .ShouldSatisfyAllConditions(errorsMessages =>
            {
                errorsMessages.Count().ShouldBe(1);
                errorsMessages.ShouldAllBe(errorMessage =>
                    errorMessage.GetString() == expectedMessage);
            });
    }
    
    [Fact]
    public async Task Validate_ShouldHaveError_WhenAccessTokenIsEmpty()
    {
        var request = RequestRecipeJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Post(REQUEST_URI, request, cancellationToken, acessToken: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        await using var respondeBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(respondeBody, cancellationToken: cancellationToken);

        var expectedMessage =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_ACESS_TOKEN_REQUIRED",
                new CultureInfo("en"));
        
        responseData.RootElement.GetProperty("errorMessages").EnumerateArray()
            .ShouldSatisfyAllConditions(errorsMessages =>
            {
                errorsMessages.Count().ShouldBe(1);
                errorsMessages.ShouldAllBe(errorMessage =>
                    errorMessage.GetString() == expectedMessage);
            });
    }
    
    [Fact]
    public async Task Validate_ShouldBeErrorResponse_WhenUserFromAcessTokenDoesExists()
    {
        var request = RequestRecipeJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Post(REQUEST_URI, request, cancellationToken, acessToken: ACESS_TOKEN_NOT_EXISTS_IN_DATABASE);

        
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        
        await using var respondeBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(respondeBody, cancellationToken: cancellationToken);
        
        var expectedMessage =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_RESOURCE_ACESS_DENIED",
                new CultureInfo("en"));
        
        responseData.RootElement.GetProperty("errorMessages").EnumerateArray()
            .ShouldSatisfyAllConditions(errorsMessages =>
            {
                errorsMessages.Count().ShouldBe(1);
                errorsMessages.ShouldAllBe(errorMessage =>
                    errorMessage.GetString() == expectedMessage);
            });
    }
    
    
}