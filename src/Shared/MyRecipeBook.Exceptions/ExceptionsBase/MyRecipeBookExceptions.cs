using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public abstract class MyRecipeBookExceptions : System.Exception
{
    public abstract HttpStatusCode GetStatusCodes();
    public abstract List<string> GetErrorMessages();

}