using MyRecipeBook.Domain.Security.Tokens;

namespace MyRecipeBook.API.Token;

public class HttpContextTokenProvider : IAcessTokenProvider
{
    private const string FormatToken = "Bearer ";

    private readonly IHttpContextAccessor _contextAccessor;

    public HttpContextTokenProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    
    public string GetToken()
    {
        var token = _contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

        return token[FormatToken.Length..];
    }
}