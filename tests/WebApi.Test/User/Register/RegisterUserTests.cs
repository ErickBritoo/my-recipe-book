using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.User.Register;

public class RegisterUserTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/users";
    
    public RegisterUserTests(MyRecipeBookApplicationFactory factory): base(factory)
    {
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;
        
        var response = await Post(REQUEST_URI, request, cancellationToken);
        
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);
        
        responseData.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        responseData.RootElement.GetProperty("tokens").GetProperty("acessToken").GetString().ShouldNotBeNullOrEmpty();
        responseData.RootElement.GetProperty("tokens").GetProperty("refreshToken").GetString().ShouldBeNullOrEmpty();
        
        var existUser = dbContext.Users.Any(user => user.Active && user.Name.Equals(request.Name) && user.Email.Equals(request.Email));
        
        existUser.ShouldBeTrue();
    }

    [Theory()]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldHaveError_WhenNameIsEmpty(string culture)
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;
        
        var cancellationToken = TestContext.Current.CancellationToken;
        
        var response = await Post(REQUEST_URI, request, cancellationToken, culture);
        
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessageException = ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_NAME_REQUIRED", new CultureInfo(culture));
        
        responseData.RootElement.GetProperty("errors").EnumerateArray().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(errorMessage => errorMessage.GetString() == expectedMessageException);
        });
    }
}