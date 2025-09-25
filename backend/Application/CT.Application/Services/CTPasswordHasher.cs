using CT.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace CT.Application.Services;

public class PasswordHasher: IPasswordHasher
{
    private const int SaltSize = 16;   // 128-bit
    private const int KeySize = 32;    // 256-bit
    private const int Iterations = 100_000; 

    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        // Generate a new random salt
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Derive a key from the password + salt
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithm,
            KeySize
        );

        // Return format: {iterations}.{salt}.{hash}
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // Split stored hash into parts
        var parts = hashedPassword.Split('.', 3);
        if (parts.Length != 3)
        {
            return false; // Invalid format
        }

        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var storedHash = Convert.FromBase64String(parts[2]);

        // Hash the incoming password using same salt + iterations
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithm,
            storedHash.Length
        );

        // Compare securely (constant time)
        return CryptographicOperations.FixedTimeEquals(hashToCompare, storedHash);
    }
}
