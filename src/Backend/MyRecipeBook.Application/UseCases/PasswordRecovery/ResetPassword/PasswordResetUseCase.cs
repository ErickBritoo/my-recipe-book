using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Repositories.VerificationCode;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.ResetPassword;

public class PasswordResetUseCase : IPasswordResetUseCase
{
    private const int VALID_MINUTES_CODE = 10;
    
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
    private readonly IVerificationCodeReadOnlyRepository _codeReadOnlyRepository;
    private readonly IVerificationCodeWriteOnlyRepository _codeWriteOnlyRepository;
    private readonly IPasswordHasher _passwordHasher;

    public PasswordResetUseCase(
        IUserReadOnlyRepository userReadOnlyRepository,
        IUserUpdateOnlyRepository userUpdateOnlyRepository,
        IVerificationCodeReadOnlyRepository codeReadOnlyRepository,
        IVerificationCodeWriteOnlyRepository codeWriteOnlyRepository,
        IPasswordHasher passwordHasher)
    {
        _userReadOnlyRepository = userReadOnlyRepository;
        _userUpdateOnlyRepository = userUpdateOnlyRepository;
        _codeReadOnlyRepository = codeReadOnlyRepository;
        _codeWriteOnlyRepository = codeWriteOnlyRepository;
        _passwordHasher = passwordHasher;
    }
    
    public async Task Execute(RequestResetRecoveryPasswordJson request)
    {
        var user = await _userReadOnlyRepository.GetByEmail(request.Email);
        
        if (user is null)
            throw new ErrorOnValidationException([ResourceMessagesExceptions.VERIFICATION_CODE_INVALID]);
        
        var verificationCode = await _codeReadOnlyRepository.Get(user.Id, request.Code, VerificationCodeType.PasswordRecovery);

        if (verificationCode is null)
            throw new ErrorOnValidationException([ResourceMessagesExceptions.VERIFICATION_CODE_INVALID]);
        
        await Validate(request, verificationCode);

        var passwordHash = _passwordHasher.HashPassword(request.NewPassword);

        await _userUpdateOnlyRepository.UpdatePassword(user.Id, passwordHash);

        await _codeWriteOnlyRepository.Delete(verificationCode);
    }


    private static async Task Validate(RequestResetRecoveryPasswordJson request, VerificationCode verificationCode)
    {
        var codeIsValid = verificationCode.CreatedAt.AddMinutes(VALID_MINUTES_CODE) >= DateTime.UtcNow;

        if (codeIsValid == false)
            throw new ErrorOnValidationException([ResourceMessagesExceptions.VERIFICATION_CODE_INVALID]);
        
        var validator = new ResetPasswordValidator();

        var res = await validator.ValidateAsync(request);

        if (res.IsValid == false)
            throw new ErrorOnValidationException(res.Errors.Select(error => error.ErrorMessage).ToList());
    }
}