using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace MyRecipeBook.Infrastructure.Migrations;

public class DatabaseMigration
{
    public static async Task ExecuteMigrations(IServiceProvider provider)
    {
        var runner = provider.GetRequiredService<IMigrationRunner>();
        
        runner.ListMigrations();
        
        runner.MigrateUp();
    }
}