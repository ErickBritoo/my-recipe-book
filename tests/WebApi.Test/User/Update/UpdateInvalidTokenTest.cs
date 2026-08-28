using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using IBM.Data.Db2;
using MyRecipeBook.Exceptions;
using Shouldly;
using Xunit;

namespace WebApi.Test.User.Update;

public class UpdateInvalidTokenTest : BaseIntegrationTest
{
    private readonly string REQUEST_URI = "users/profile";
    private readonly string ACESS_TOKEN_NOT_EXISTS_IN_DATABASE;

    public UpdateInvalidTokenTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        ACESS_TOKEN_NOT_EXISTS_IN_DATABASE = factory.Token_User_Not_Found_In_Database;
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenAccessTokenIsInvalid()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Put(REQUEST_URI, request, "invalidAcessToken", cancellationToken);

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
        var request = RequestUpdateUserJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Put(REQUEST_URI, request, string.Empty, cancellationToken);

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
        var request = RequestChangePasswordJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Put(REQUEST_URI, request, ACESS_TOKEN_NOT_EXISTS_IN_DATABASE, cancellationToken);

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