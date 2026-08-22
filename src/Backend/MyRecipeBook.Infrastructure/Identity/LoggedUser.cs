using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.DataAcess;
using MyRecipeBook.Infrastructure.Security.Tokens.AcessToken;

namespace MyRecipeBook.Infrastructure.Identity;

internal class LoggedUser : ILoggedUser
{
    private readonly MyRecipeBookDbContext _dbContext;
    private readonly IAcessTokenProvider _acessTokenProvider;

    public LoggedUser(MyRecipeBookDbContext dbContext, IAcessTokenProvider acessTokenProvider)
    {
        _dbContext = dbContext;
        _acessTokenProvider = acessTokenProvider;
    }
    
    public async Task<User> Get()
    {
        var userId = GetUserId();
        
        var user =  await _dbContext.Users.AsNoTracking().FirstAsync(user => user.Active && user.Id == userId);

        return user;
    }

    public Guid GetUserId()
    {
        var token = _acessTokenProvider.GetToken();

        var handler = new JsonWebTokenHandler();

        var subject =  handler.ReadJsonWebToken(token).Subject!;
        
        return Guid.Parse(subject);
    }
}