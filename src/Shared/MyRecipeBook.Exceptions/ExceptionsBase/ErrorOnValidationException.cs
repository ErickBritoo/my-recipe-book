namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class ErrorOnValidationException : MyRecipeBookExceptions
{
    private readonly List<string> _errorMessages;

    public ErrorOnValidationException(List<string> errorMessages)
    {
        _errorMessages = errorMessages;
    }
    
    public List<string> ErrorMessages => _errorMessages;
}