using CommonTestUtilities.Requests;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using Shouldly;
using Xunit;

namespace Validators.Tests.User.Register;

public class RegisterUserValidatorTest
{
    [Fact]
    public void Sucess()
    {
        // Arrange
        var request = RequestRegisterUserJsonBuilder.Build(); 
        var validator = new RegisterUserValidator();
        
        // Act 
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Xunit.Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("                      ")]
    public void Validate_Error_WhenNameEmpty(string name)
    {
        // Arrange
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = name;

        var validator = new RegisterUserValidator();
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_NAME_REQUIRED));
        });
    }

    [Fact]
    public void Validate_Error_WhenEmailEmpty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = string.Empty;

        var validator = new RegisterUserValidator();

        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_EMAIL_REQUIRED));
        });
    }

    [Fact]
    public void Validate_Error_WhenEmailInvalid()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = "email.com";

        var validator = new RegisterUserValidator();

        var result = validator.Validate(request);
        
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_EMAIL_INVALID));
        });
    }

    [Fact]
    public void Validate_Error_WhenPasswordEmpty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Password = string.Empty;

        var validator = new RegisterUserValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.VALIDATION_PASSWORD_REQUIRED));
        });
    }
}