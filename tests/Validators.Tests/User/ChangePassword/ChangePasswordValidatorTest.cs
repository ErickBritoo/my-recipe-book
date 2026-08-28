using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.User.ChangePassword;

public class ChangePasswordValidatorTest
{
    [Fact]
    public void Sucess()
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        var validator = new ChangePasswordValidator();

        var res = validator.Validate(request);

        res.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("            ")]
    public void Validate_Error_WhenPasswordEmpty(string password)
    {
        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = password;
        var validator = new ChangePasswordValidator();

        var res = validator.Validate(request);

        res.IsValid.ShouldBeFalse();
        res.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_PASSWORD_REQUIRED));
        });
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Validate_Error_WhenPasswordLengthMin(int passwordLength)
    {
        var request = RequestChangePasswordJsonBuilder.Build(passwordLength);
        var validator = new ChangePasswordValidator();

        var res = validator.Validate(request);

        res.IsValid.ShouldBeFalse();
        res.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_PASSWORD_MIN_LENGTH));
        });
    }
}