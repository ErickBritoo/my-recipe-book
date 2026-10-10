using System.Security.Cryptography;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Repositories.VerificationCode;

namespace MyRecipeBook.Application.UseCases.PasswordRecovery.RequestCode;

public class PasswordRecoveryUseCase : IPasswordRecoveryUseCase
{
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IVerificationCodeWriteOnlyRepository _verificationCodeWriteOnlyRepository;
    private readonly IUnityOfWork _unityOfWork;
    
    public PasswordRecoveryUseCase(
        IUserReadOnlyRepository userReadOnlyRepository, 
        IVerificationCodeWriteOnlyRepository verificationCodeWriteOnlyRepository,
        IUnityOfWork unityOfWork)
    {
        _userReadOnlyRepository = userReadOnlyRepository;
        _verificationCodeWriteOnlyRepository = verificationCodeWriteOnlyRepository;
        _unityOfWork = unityOfWork;
    }
    
    public async Task Execute(RequestPasswordRecovery request)
    {
        var user = await _userReadOnlyRepository.GetByEmail(request.Email);

        if (user == null)
            return;

        var code = RandomNumberGenerator.GetInt32(1, 1_000_000);

        var verificationCode = new VerificationCode()
        {
            Type = VerificationCodeType.PasswordRecovery,
            Code = code.ToString("D6"),
            UserId = user.Id
        };
        
        // send to email

        await _verificationCodeWriteOnlyRepository.Add(verificationCode);

        await _unityOfWork.Commit();
    }
}