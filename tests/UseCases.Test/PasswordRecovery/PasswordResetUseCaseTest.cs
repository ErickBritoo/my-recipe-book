using System.Net;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using Shouldly;
using Xunit;

namespace UseCases.Test.PasswordRecovery;

public class PasswordResetUseCaseTest
{
    [Fact]
    public async Task Sucess()
    {
        var (user, _) = UserBuilder.Build();
        
        var verificationCode = VerificationCodeBuilder.Build(user.Id);
        var request = RequestResetRecoveryPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = verificationCode.Code;
        
        var useCase = CreateUseCase(user, verificationCode);

        await useCase.Execute(request).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Validate_ThrowException_WhenUserNotExist()
    {
        var (user, _) = UserBuilder.Build();

        var verificationCode = VerificationCodeBuilder.Build(user.Id);
        
        var request = RequestResetRecoveryPasswordJsonBuilder.Build();

        var useCase = CreateUseCase(verificationCode: verificationCode);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        
        var messageException = exception.GetErrorMessages().ShouldHaveSingleItem();
        messageException.ShouldBe(ResourceMessagesExceptions.VERIFICATION_CODE_INVALID);
    }
    
    [Fact]
    public async Task Validate_ThrowException_WhenVerificationCodeNotExist()
    {
        var (user, _) = UserBuilder.Build();
        
        var request = RequestResetRecoveryPasswordJsonBuilder.Build();
        request.Email = user.Email;
        
        var useCase = CreateUseCase(user);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        
        var messageException = exception.GetErrorMessages().ShouldHaveSingleItem();
        messageException.ShouldBe(ResourceMessagesExceptions.VERIFICATION_CODE_INVALID);
    }

    [Fact]
    public async Task Validate_ThrowException_WhenVerificationCodeExpired()
    {
        var (user, _) = UserBuilder.Build();

        var verificationCode = VerificationCodeBuilder.Build(user.Id);
        verificationCode.CreatedAt = DateTime.UtcNow.AddMinutes(-11);
        
        var request = RequestResetRecoveryPasswordJsonBuilder.Build();
        request.Email = user.Email;
        request.Code = verificationCode.Code;
        
        var useCase = CreateUseCase(user, verificationCode);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        
        var messageException = exception.GetErrorMessages().ShouldHaveSingleItem();
        messageException.ShouldBe(ResourceMessagesExceptions.VERIFICATION_CODE_INVALID);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task Validate_ThrowException_WhenPasswordInvalid(int passwordLength)
    {
        var (user, _) = UserBuilder.Build();

        var verificationCode = VerificationCodeBuilder.Build(user.Id);
        
        var request = RequestResetRecoveryPasswordJsonBuilder.Build(passwordLength);
        request.Email = user.Email;
        request.Code = verificationCode.Code;
        
        var useCase = CreateUseCase(user, verificationCode);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        
        exception.GetStatusCodes().ShouldBe(HttpStatusCode.BadRequest);
        
        var messageException = exception.GetErrorMessages().ShouldHaveSingleItem();
        messageException.ShouldBe(ResourceMessagesExceptions.VALIDATION_PASSWORD_MIN_LENGTH);
    }
    
    
    private IPasswordResetUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User? user = null,
        VerificationCode? verificationCode = null)
    {
        var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
        var userUpdateOnlyRepository = UserUpdateOnlyRepositoryBuilder.Build();
        var passwordHashed = new PasswordHasherBuilder();
        var codeWriteOnlyRepository = VerificationCodeWriteOnlyRepositoryBuilder.Build();
        var codeReadOnlyRepositoryBuilder = new IVerificationCodeReadOnlyRepositoryBuilder();

        if (user is not null)
            userReadOnlyRepositoryBuilder.GetByEmail(user);

        if (verificationCode is not null)
            codeReadOnlyRepositoryBuilder.Get(verificationCode);

        return new PasswordResetUseCase(
            userReadOnlyRepositoryBuilder.Build(),
            userUpdateOnlyRepository,
            codeReadOnlyRepositoryBuilder.Build(),
            codeWriteOnlyRepository,
            passwordHashed.Build());
    }
}