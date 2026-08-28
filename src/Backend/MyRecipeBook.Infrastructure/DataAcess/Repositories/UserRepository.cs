using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace MyRecipeBook.Infrastructure.DataAcess.Repositories;

internal sealed class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository, IUserUpdateOnlyRepository
{
    private readonly MyRecipeBookDbContext _dbContext;

    public UserRepository(MyRecipeBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(User user) => await _dbContext.Users.AddAsync(user);

    public async Task<bool> ExistActiveUserWithId(Guid userId)
    {
        return await _dbContext.Users.AnyAsync(user => user.Active && user.Id == userId);
    }

    public async Task<bool> ExistActiveUserWithEmail(string email) => await _dbContext.Users.AnyAsync(user => user.Active && user.Email.Equals(email));

    // Single / SingleOrDefault:
    // - Esperam encontrar no máximo um User que atenda às condições.
    // - Lançam exceção se mais de um User for encontrado.
    // - Single: lança exceção se nenhum User for encontrado.
    // - SingleOrDefault: retorna null se nenhum User for encontrado.
    //
    // First / FirstOrDefault:
    // - Buscam o primeiro User que atenda às condições.
    // - Não lançam exceção caso existam vários Users.
    // - First: lança exceção se nenhum User for encontrado.
    // - FirstOrDefault: retorna null se nenhum User for encontrado.
    // 
    // Single/SingleOrDefault -> usar quando esperamos que exista no máximo um registro.
    // First/FirstOrDefault   -> usar quando podemos ter vários registros, mas queremos apenas o primeiro.\

    // AsNoTracking: 
    // Informa ao EF para tornar a entidade não rastreável
    // EF não ficará a observar a entidade
    // Ganho de desempenho, mas com isso não conseguimos salvar alterações nessa entidade
    // Usar quando existir em contexto de somente leitura
    public async Task<User?> GetByEmail(string email) => await _dbContext.Users.AsNoTracking()
        .SingleOrDefaultAsync(user => user.Active & user.Email.Equals(email));

    public void UpdateProfile(User user)
    {
        _dbContext.Users.Attach(user);

        _dbContext.Entry(user).Property(u => u.Name).IsModified = true;
        _dbContext.Entry(user).Property(u => u.Email).IsModified = true;
    }
}