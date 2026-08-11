using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.PasswordHashing;
using MyRecipeBook.Infrastructure.DataAcess;
using MyRecipeBook.Infrastructure.DataAcess.Repositories;
using MyRecipeBook.Infrastructure.Security.PasswordHashing;

namespace MyRecipeBook.Infrastructure;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddInfrastructure(IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
            
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            
            services.AddDbContext<MyRecipeBookDbContext>(config =>
            {
                var connectionString = configuration.GetConnectionString("DbConnection")!;
                
                config.UseMySQL(connectionString);
            });
            
            services.AddScoped<IUnityOfWork, UnityOfWork>();
            
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
    }
}