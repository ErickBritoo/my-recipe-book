using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;

[assembly: InternalsVisibleTo("WebApi.Test")]

namespace MyRecipeBook.Infrastructure.DataAcess;

internal class MyRecipeBookDbContext : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Recipe> Recipes { get; init; }

    public MyRecipeBookDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<RecipeInstruction>().ToTable("RecipeInstructions");
        modelBuilder.Entity<RecipeIngredient>().ToTable("RecipeIngredients");

        modelBuilder.Entity<RecipeDishType>().ToTable("RecipeDishTypes")
            .Property(dishType => dishType.Type)
            .HasConversion<string>();
        
        modelBuilder.Entity<Recipe>().Property(recipe => recipe.CookTime).HasConversion<string>();
    }
}