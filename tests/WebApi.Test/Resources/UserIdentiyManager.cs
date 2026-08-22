namespace WebApi.Test.Resources;

public class UserIdentiyManager
{
    private readonly MyRecipeBook.Domain.Entities.User _user;
    private readonly string _password;
    private readonly string _acessToken;
    
    public UserIdentiyManager(MyRecipeBook.Domain.Entities.User user, string password, string acessToken)
    {
        _user = user;
        _password = password;
        _acessToken = acessToken;
    }

    public Guid GetId() => _user.Id;
    public string GetName() => _user.Name;
    public string GetEmail() => _user.Email;
    public string GetPassword() => _password;
    public string GetAcessToken() => _acessToken;
}