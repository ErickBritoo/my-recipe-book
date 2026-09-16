using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Identity;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructure.DataAcess;
using MyRecipeBook.Infrastructure.DataAcess.Repositories;
using MyRecipeBook.Infrastructure.Identity;
using MyRecipeBook.Infrastructure.Security.PasswordHashing;
using MyRecipeBook.Infrastructure.Security.Tokens.AcessToken;

namespace MyRecipeBook.Infrastructure;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure(IConfiguration configuration)
        {
            services.AddRepositories();
            services.AddTokensHandler(configuration);
            
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            services.AddScoped<ILoggedUser, LoggedUser>();

            services.AddDbContext<MyRecipeBookDbContext>(config =>
            {
                var connectionString = configuration.GetConnectionString("DbConnection")!;

                config.UseMySQL(connectionString);
            });

            services.AddFluentMigratorCore().ConfigureRunner(rb =>
            {
                rb.AddMySql5().WithGlobalConnectionString(serviceProvider =>
                    {
                        var scope = serviceProvider.GetRequiredService<IConfiguration>();

                        return scope.GetConnectionString("DbConnection");
                    })
                    .ScanIn(Assembly.Load("MyRecipeBook.Infrastructure")).For.All();
            });
        }

        private void AddRepositories()
        {
            services.AddScoped<IUnityOfWork, UnityOfWork>();

            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
            
            services.AddScoped<IRecipeWriteOnlyRepository, RecipeRepository>();
            services.AddScoped<IRecipeReadOnlyRepository, RecipeRepository>();
            services.AddScoped<IRecipeDeleteOnlyRepository, RecipeRepository>();
            services.AddScoped<IRecipeUpdateOnlyRepository, RecipeRepository>();
        }
        
        private void AddTokensHandler(IConfiguration configuration)
        {
            services.AddScoped<IAcessTokenGenerator>(_ =>
            {
                var expirationTimeMinutes = configuration.GetValue<uint>("Jwt:ExpirationTimeMinutes");
                var signatureKey = configuration.GetValue<string>("Jwt:SigningKey")!;

                return new JwtTokenHandler(expirationTimeMinutes, signatureKey);
            });
        }
    }
}