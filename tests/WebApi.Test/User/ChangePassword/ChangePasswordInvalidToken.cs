using System.Net;
using CommonTestUtilities.Requests;
using Shouldly;
using Xunit;

namespace WebApi.Test.User.ChangePassword;

public class ChangePasswordInvalidToken : BaseIntegrationTest
{
    private const string REQUEST_URI = "users/password";
    private readonly string ACESS_TOKEN_NOT_EXISTS_IN_DATABASE;
    
    public ChangePasswordInvalidToken(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        ACESS_TOKEN_NOT_EXISTS_IN_DATABASE = factory.Token_User_Not_Found_In_Database;
    }

    [Fact]
    public async Task Validate_ShouldHaveError_WhenAccessTokenIsInvalid()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Put(REQUEST_URI, request, "invalidToken", cancellationToken);
        
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Validate_ShouldHaveError_WhenAcessTokenEmpty()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;
        
        var response = await Put(REQUEST_URI, request, string.Empty, cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ShouldBeErrorResponse_WhenUserFromAcessTokenDoesExists()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;
        
        var response = await Put(REQUEST_URI, request, ACESS_TOKEN_NOT_EXISTS_IN_DATABASE, cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}