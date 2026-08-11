using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CommonTestUtilities.Requests;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Infrastructure.DataAcess;
using Shouldly;
using WebApi.Test.InlineData;
using Xunit;

namespace WebApi.Test.User.Register;

public class RegisterUserTests : IClassFixture<MyRecipeBookApplicationFactory>
{
    private readonly HttpClient _httpClient;
    private readonly MyRecipeBookDbContext _dbContext;
    private const string REQUEST_URI = "/users";
    
    public RegisterUserTests(MyRecipeBookApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
        _dbContext = factory.Services.GetRequiredService<MyRecipeBookDbContext>();
    }

    [Fact]
    public async Task Sucess()
    {
        
        var request = RequestRegisterUserJsonBuilder.Build();
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request, cancellationToken);
        
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);
        
        responseData.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        responseData.RootElement.GetProperty("tokens").GetProperty("acessToken").GetString().ShouldBeEmpty();
        responseData.RootElement.GetProperty("tokens").GetProperty("refreshToken").GetString().ShouldBeEmpty();
        
        var existUser = _dbContext.Users.Any(user => user.Active && user.Name.Equals(request.Name) && user.Email.Equals(request.Email));
        
        existUser.ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldHaveError_WhenNameIsEmpty(string culture)
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;
        var cancellationToken = TestContext.Current.CancellationToken;

        
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);
        
        var response = await _httpClient.PostAsJsonAsync(REQUEST_URI, request, cancellationToken);
        
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessageException = ResourceMessagesExceptions.ResourceManager.GetString("VALIDATION_NAME_REQUIRED", new CultureInfo(culture));
        
        responseData.RootElement.GetProperty("errors").EnumerateArray().ShouldSatisfyAllConditions(errors =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(error => error.GetString()!.Equals(expectedMessageException));
        });
    }
}