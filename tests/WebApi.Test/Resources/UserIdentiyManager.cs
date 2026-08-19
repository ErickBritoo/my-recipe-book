namespace WebApi.Test.Resources;

public class UserIdentiyManager
{
    private readonly MyRecipeBook.Domain.Entities.User _user;
    private readonly string _password;

    public UserIdentiyManager(MyRecipeBook.Domain.Entities.User user, string password)
    {
        _user = user;
        _password = password;
    }

    public Guid GetID() => _user.Id;
    public string GetName() => _user.Name;
    public string GetEmail() => _user.Email;
    public string GetPassword() => _password;
}