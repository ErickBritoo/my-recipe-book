using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;

[assembly: InternalsVisibleTo("WebApi.Test")]

namespace MyRecipeBook.Infrastructure.DataAcess;

internal class MyRecipeBookDbContext : DbContext
{
    public DbSet<User> Users { get; init; }
    
    public MyRecipeBookDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) {}
    
}