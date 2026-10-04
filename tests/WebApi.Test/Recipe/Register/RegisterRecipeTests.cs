using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.Recipe.Register;

public class RegisterRecipeTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/recipes";
    private readonly UserIdentiyManager _user1;
    
    public RegisterRecipeTests(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }

    [Fact]
    public async Task Sucess()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Post(REQUEST_URI, request, cancellationToken, acessToken: _user1.GetAcessToken());
        
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);
        
        responseData.RootElement.GetProperty("title").GetString().ShouldBe(request.Title);
        
        var recipeId = responseData.RootElement.GetProperty("id").GetGuid();

        var recipeExists = await dbContext.Recipes.AnyAsync(recipe =>
            recipe.Id == recipeId &&
            recipe.Active &&
            recipe.Title.Equals(request.Title) &&
            recipe.UserId == _user1.GetId(),
            cancellationToken);

        recipeExists.ShouldBeTrue();
    }


    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldHaveError_WhenTitleIsEmpty(string culture)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;
        
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Post(REQUEST_URI, request, cancellationToken, culture, _user1.GetAcessToken());
        
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessageException = ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_TITLE_REQUIRED", new CultureInfo(culture));
        
        responseData.RootElement.GetProperty("errorMessages").EnumerateArray().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(errorMessage => errorMessage.GetString() == expectedMessageException);
        });
    }
}