using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class ErrorOnValidationException : MyRecipeBookExceptions
{
    private readonly List<string> _errorMessages;
    
    public ErrorOnValidationException(List<string> errorMessages)
    {
        _errorMessages = errorMessages;
    }
    
    public override HttpStatusCode GetStatusCodes() => HttpStatusCode.BadRequest;

    public override List<string> GetErrorMessages() => _errorMessages;
}