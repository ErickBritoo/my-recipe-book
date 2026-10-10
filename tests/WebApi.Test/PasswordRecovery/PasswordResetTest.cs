using System.Globalization;
using System.Net;
using System.Text.Json;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Exceptions;
using Shouldly;
using WebApi.Test.InlineData;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test.PasswordRecovery;

public class PasswordResetTest : BaseIntegrationTest
{
    private const string REQUEST_URI = "authenticate/password/reset";
    private readonly UserIdentiyManager _user1;

    public PasswordResetTest(MyRecipeBookApplicationFactory factory) : base(factory)
    {
        _user1 = factory.User1;
    }

    [Fact]
    public async Task Sucess()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var verificationCode = VerificationCodeBuilder.Build(_user1.GetId());

        dbContext.VerificationCodes.Add(verificationCode);
        await dbContext.SaveChangesAsync(cancellationToken);

        var request = RequestResetRecoveryPasswordJsonBuilder.Build();
        request.Email = _user1.GetEmail();
        request.Code = verificationCode.Code;

        var response = await Post(REQUEST_URI, request, cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        responseBody.ShouldBe(string.Empty);

        var verificationCodeExistsInDatabase = await dbContext.VerificationCodes.AnyAsync(code =>
            code.UserId == _user1.GetId() &&
            code.Id == verificationCode.Id, cancellationToken);

        verificationCodeExistsInDatabase.ShouldBe(false);
    }

    [Theory]
    [ClassData(typeof(CultureInlineData))]
    public async Task Validate_ShouldHaveError_WhenVerificationCode_NotExist(string culture)
    {
        var request = RequestResetRecoveryPasswordJsonBuilder.Build();
        request.Email = _user1.GetEmail();

        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await Post(REQUEST_URI, request, cancellationToken, culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

        var expectedMessageException =
            ResourceMessagesExceptions.ResourceManager.GetString("VERIFICATION_CODE_INVALID", new CultureInfo(culture));

        var messageResponse = responseData.RootElement.GetProperty("errorMessages").EnumerateArray()
            .ShouldHaveSingleItem();

        messageResponse.GetString().ShouldBe(expectedMessageException);
    }
}