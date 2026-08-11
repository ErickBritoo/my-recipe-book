namespace MyRecipeBook.Communication.Responses;

public class ResponseErrorJson
{
    public List<string> Errors { get; set; }

    public ResponseErrorJson(List<string> errors) => Errors = errors;
    public ResponseErrorJson(string error) => Errors = [error];
}