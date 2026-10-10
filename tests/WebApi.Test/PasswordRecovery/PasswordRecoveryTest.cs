using System.Net;
using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Enums;
using Shouldly;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.PasswordRecovery;

public class PasswordRecoveryTest : BaseIntegrationTest
{
    private const string REQUEST_URI = "authenticate/password/recovery";
    private readonly UserIdentiyManager _user1;
    
    
    public PasswordRecoveryTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }

    [Fact]
    public async Task Sucess()
    {
        var request = new RequestPasswordRecovery()
        {
            Email = _user1.GetEmail()
        };

        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Post(REQUEST_URI, request, cancellationToken);
        
        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        responseBody.ShouldBe(string.Empty);

        var codeGenerated = dbContext.VerificationCodes.Any(code =>
            code.UserId == _user1.GetId() &&
            code.Type == VerificationCodeType.PasswordRecovery);
        
        codeGenerated.ShouldBeTrue();
    }
    
    [Fact]
    public async Task Success_WhenUserDoesNotExist()
    {
        var request = RequestPasswordRecoveryJsonBuilder.Build();

        var cancellationToken = TestContext.Current.CancellationToken;
        
        var codeBeforeCount = await dbContext.VerificationCodes.AsNoTracking().CountAsync(cancellationToken);
        
        var response = await Post(REQUEST_URI, request, cancellationToken);
        
        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        responseBody.ShouldBe(string.Empty);
        
        var codeAfterCount  = await dbContext.VerificationCodes.AsNoTracking().CountAsync(cancellationToken);

        codeAfterCount.ShouldBe(codeBeforeCount);
    }

}