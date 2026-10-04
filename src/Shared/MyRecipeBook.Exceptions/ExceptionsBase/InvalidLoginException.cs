using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class InvalidLoginException : MyRecipeBookExceptions
{
    public override HttpStatusCode GetStatusCodes() => HttpStatusCode.Unauthorized;

    public override List<string> GetErrorMessages() => [ResourceMessagesExceptions.VALIDATION_LOGIN_INVALID];
}