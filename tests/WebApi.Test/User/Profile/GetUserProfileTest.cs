using System.Text.Json;
using Shouldly;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.User.Profile;

public class GetUserProfileTest : BaseIntegrationTest
{
    private UserIdentiyManager _user1;
    private const string REQUEST_URI = "users";
    
    public GetUserProfileTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }
    
    [Fact]
    public async Task Sucess()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Get(REQUEST_URI, cancellationToken, acessToken: _user1.GetAcessToken());

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);
        
        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);
        
        responseData.RootElement.GetProperty("name").GetString().ShouldBe(_user1.GetName());
        responseData.RootElement.GetProperty("email").GetString().ShouldBe(_user1.GetEmail());
    }
}