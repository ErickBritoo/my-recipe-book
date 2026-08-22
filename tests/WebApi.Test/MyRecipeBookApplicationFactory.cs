using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.DataAcess;
using Testcontainers.MySql;
using WebApi.Test.Resources;
using Xunit;

namespace WebApi.Test;

public class MyRecipeBookApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public UserIdentiyManager User1 { get; private set; }
    private readonly MySqlContainer _mySqlContainer;

    public MyRecipeBookApplicationFactory()
    {
        _mySqlContainer = new MySqlBuilder("mysql:8.0")
            .WithDatabase("meulivrodereceitas")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _mySqlContainer.GetConnectionString();

        builder
            .UseEnvironment("Tests")
            .ConfigureAppConfiguration((_, configuration) =>
            {
                var parameters = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DbConnection"] = connectionString
                };

                configuration.AddInMemoryCollection(parameters);
            });
    }

    public async ValueTask InitializeAsync()
    {
        await _mySqlContainer.StartAsync();

        await using var scope =  Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MyRecipeBookDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var acessTokenGenerator = scope.ServiceProvider.GetRequiredService<IAcessTokenGenerator>();
        
        var (user, password) = UserBuilder.Build();

        user.Password = passwordHasher.HashPassword(password);
        var acessToken = acessTokenGenerator.Generate(user);
        
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        User1 = new UserIdentiyManager(user, password, acessToken);
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _mySqlContainer.DisposeAsync();
    }
}