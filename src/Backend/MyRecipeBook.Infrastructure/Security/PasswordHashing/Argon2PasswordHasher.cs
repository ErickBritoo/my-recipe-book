using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using MyRecipeBook.Domain.Security.PasswordHashing;

namespace MyRecipeBook.Infrastructure.Security.PasswordHashing;

public class Argon2PasswordHasher : IPasswordHasher
{
    private const int DEGREE_OF_PARALLELISM = 1;
    private const int ITERATIONS = 2;
    private const int MEMORY_SIZE = 20 * 1024; // 20MB
    private const int SALT_SIZE = 16;
    private const int HASH_SIZE = 32;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SALT_SIZE);

        var hash = HashPassword(password, salt);

        var combinedBytes = new byte[HASH_SIZE + SALT_SIZE];

        salt.CopyTo(combinedBytes);
        hash.CopyTo(combinedBytes, SALT_SIZE);

        return Convert.ToBase64String(combinedBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var hashAndSaltBytes = Convert.FromBase64String(passwordHash);

        var hash = new byte[HASH_SIZE];
        var salt = new byte[SALT_SIZE];

        Array.Copy(hashAndSaltBytes, salt, SALT_SIZE);
        Array.Copy(hashAndSaltBytes, SALT_SIZE, hash, 0, HASH_SIZE);

        var newHash = HashPassword(password, salt);

        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }

    private byte[] HashPassword(string password, byte[] salt)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var hashAlgorithm = new Argon2id(passwordBytes)
        {
            DegreeOfParallelism = DEGREE_OF_PARALLELISM, // Dividir o processamento do algoritmo com paralelismo(qtd threads)
            Iterations = ITERATIONS, // Número de vezes que a senha é passada no algoritmo
            MemorySize = MEMORY_SIZE, // Quantidade de memória RAM a ser utilizada para cada senha a ser hasheada
            Salt = salt // Valores a serem adicionados a senha para gerar diferentes hashes
        };

        var hash = hashAlgorithm.GetBytes(HASH_SIZE);

        return hash;
    }
}