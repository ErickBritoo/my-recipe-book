namespace MyRecipeBook.Communication.Responses;

public class ResponseErrorJson
{
    public List<string> ErrorMessages { get; set; }
    public bool AcessTokenExpired { get; private set;  }
    
    public ResponseErrorJson(List<string> errorMessages) => ErrorMessages = errorMessages;
    public ResponseErrorJson(string errorMessage) => ErrorMessages = [errorMessage];

    public ResponseErrorJson(string errorMessage, bool acessTokenExpired)
    {
        ErrorMessages = [errorMessage];
        AcessTokenExpired = acessTokenExpired;
    }
}