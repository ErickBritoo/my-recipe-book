using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class NotFoundException: MyRecipeBookExceptions
{
    private readonly List<string> _errorMessages;

    public NotFoundException(string errorMessage)
    {
        _errorMessages = [errorMessage];
    }

    public override List<string> GetErrorMessages() => _errorMessages;
    public override HttpStatusCode GetStatusCodes() => HttpStatusCode.NotFound;
}