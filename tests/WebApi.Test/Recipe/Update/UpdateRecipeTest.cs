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

namespace WebApi.Test.Recipe.Update;

public class UpdateRecipeTest : BaseIntegrationTest
{
    private const string REQUEST_URI = "/recipes";
    private readonly UserIdentiyManager _user1;
    
    
    public UpdateRecipeTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }

    [Fact]
    public async Task Sucess()
    {
        var recipe = _user1.GetRecipe();
        var request = RequestRecipeJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Put($"{REQUEST_URI}/{recipe.Id}", request, _user1.GetAcessToken(), cancellationToken);
        
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var updatedRecipe = await dbContext.Recipes
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == recipe.Id, cancellationToken);

        updatedRecipe.ShouldNotBeNull();
        
        updatedRecipe.Title.ShouldBe(request.Title);
        updatedRecipe.UserId.ShouldBe(recipe.UserId);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldHaveError_WhenTitleIsEmpty(string culture)
    {
        var recipe = _user1.GetRecipe();
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;
        
        var cancellationToken = TestContext.Current.CancellationToken;
        
        var response = await Put($"{REQUEST_URI}/{recipe.Id}", request, _user1.GetAcessToken(), cancellationToken, culture);

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
    
    [Fact]
    public async Task Validate_ShouldHaveError_WhenRecipeNotFound()
    {
        var request = RequestRecipeJsonBuilder.Build();
        
        var cancellationToken = TestContext.Current.CancellationToken;
        
        var response = await Put($"{REQUEST_URI}/{Guid.CreateVersion7()}", request, _user1.GetAcessToken(), cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        
        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessageException = ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_NOT_FOUND_RECIPE", new CultureInfo("en"));
        
        responseData.RootElement.GetProperty("errorMessages").EnumerateArray().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(errorMessage => errorMessage.GetString() == expectedMessageException);
        });    
    }
}