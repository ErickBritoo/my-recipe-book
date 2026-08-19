using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Domain.Security.Tokens;

public interface IAcessTokenGenerator
{
    public string Generate(User user);
}