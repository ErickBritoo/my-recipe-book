using System.Globalization;
using System.Net;
using System.Text.Json;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.Recipe.GetById;

public class GetByIdRecipeTest : BaseIntegrationTest
{
    private const string REQUEST_URI = "/recipes";
    private readonly UserIdentiyManager _user1;
    
    public GetByIdRecipeTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }

    [Fact]
    public async Task Sucess()
    {
        var recipe = _user1.GetRecipe();
        
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Get($"{REQUEST_URI}/{recipe.Id}", cancellationToken, acessToken: _user1.GetAcessToken());
        
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);
        
        responseData.RootElement.GetProperty("title").GetString().ShouldBe(recipe.Title);
        responseData.RootElement.GetProperty("id").GetGuid().ShouldBe(recipe.Id);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldHaveError_WhenRecipeNull(string culture)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Get($"{REQUEST_URI}/{Guid.CreateVersion7()}", cancellationToken, culture,
            _user1.GetAcessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessageException =
            ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_NOT_FOUND_RECIPE",
                new CultureInfo(culture));
        
        responseData.RootElement.GetProperty("errorMessages").EnumerateArray().ShouldSatisfyAllConditions(errorsMessages =>
        {
            errorsMessages.Count().ShouldBe(1);
            errorsMessages.ShouldContain(errorMessage => errorMessage.GetString() == expectedMessageException);
        });
    }
    
}