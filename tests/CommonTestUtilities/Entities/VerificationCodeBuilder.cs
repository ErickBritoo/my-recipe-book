using Bogus;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Enums;

namespace CommonTestUtilities.Entities;

public static class VerificationCodeBuilder
{
    public static VerificationCode Build(Guid userId)
    {
        return new Faker<VerificationCode>()
            .RuleFor(verificationCode => verificationCode.Code, f => f.Random.Int(1, 999_999).ToString("D6"))
            .RuleFor(verificationCode => verificationCode.CreatedAt, _ => DateTime.UtcNow)
            .RuleFor(verificationCode => verificationCode.UserId, _ => userId)
            .RuleFor(verificationCode => verificationCode.Type, _  => VerificationCodeType.PasswordRecovery);
    }
}