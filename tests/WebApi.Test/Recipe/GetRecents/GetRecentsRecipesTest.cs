using System.Net;
using System.Text.Json;
using Shouldly;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.Recipe.GetRecents;

public class GetRecentsRecipesTest : BaseIntegrationTest
{
    private const string REQUEST_URI = "/recipes/recent";
    private readonly UserIdentiyManager _user1;

    public GetRecentsRecipesTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }

    [Fact]
    public async Task Sucess()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Get(REQUEST_URI, cancellationToken, acessToken: _user1.GetAcessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var recipeExpected = _user1.GetRecipe();

        var recipes = responseData.RootElement
            .GetProperty("recipes")
            .EnumerateArray()
            .ToList();
        
       recipes.Count.ShouldBe(1);
       recipes.ShouldContain(recipe =>
            recipe.GetProperty("id").GetGuid() == recipeExpected.Id &&
            recipe.GetProperty("title").GetString() == recipeExpected.Title
        );
    }
}