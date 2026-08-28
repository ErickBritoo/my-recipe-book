using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.User.Update;

public class UpdateUserValidatorTest
{
    [Fact]
    public void Sucess()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        var validator = new UpdateUserValidator();

        var res = validator.Validate(request);

        res.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("                             ")]
    public void Validate_Error_WhenNameEmpty(string name)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = name;
        var validator = new UpdateUserValidator();

        var res = validator.Validate(request);

        res.IsValid.ShouldBeFalse();
        res.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_NAME_REQUIRED));
        });
    }

    [Fact]
    public void Validate_Error_WhenEmailEmpty()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = string.Empty;
        var validator = new UpdateUserValidator();

        var res = validator.Validate(request);

        res.IsValid.ShouldBeFalse();
        res.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_EMAIL_REQUIRED));
        });
    }

    [Fact]
    public void Validate_Error_WhenEmailInvalid()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = "email.com";
        var validator = new UpdateUserValidator();

        var res = validator.Validate(request);

        res.IsValid.ShouldBeFalse();
        res.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error =>
                error.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_EMAIL_INVALID));
        });
    }
}