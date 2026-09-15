using System.Globalization;
using System.Net;
using System.Text.Json;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.Recipe.DeleteById;

public class DeleteRecipeByIdTest : BaseIntegrationTest
{
    private const string REQUEST_URI = "/recipes";
    private readonly UserIdentiyManager _user1;
    
    public DeleteRecipeByIdTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }

    [Fact]
    public async Task Sucess()
    {
        var recipeId = _user1.GetRecipe().Id;

        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Delete($"{REQUEST_URI}/{recipeId}", cancellationToken, acessToken: _user1.GetAcessToken());
        
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task Validate_ShouldHaveError_WhenRecipeNotFound()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Delete($"{REQUEST_URI}/{Guid.CreateVersion7()}", cancellationToken, acessToken: _user1.GetAcessToken());
        
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);
        
        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessageException =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_NOT_FOUND_RECIPE", new CultureInfo("en"));
        
        responseData.RootElement.GetProperty("errorMessages").EnumerateArray().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count().ShouldBe(1);
            errorsMessages.ShouldContain(errorMessage => errorMessage.GetString() == expectedMessageException);
        });
    }

}