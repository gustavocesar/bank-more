using System.Security.Cryptography;

namespace ContaCorrente.Domain.ValueObjects;

public sealed record Senha
{
    private Senha(string hash, string salt)
    {
        Hash = hash;
        Salt = salt;
    }

    public string Hash { get; }

    public string Salt { get; }

    public static Senha Criar(string valor)
    {
        var salt = GenerateSalt();
        var hash = HashPassword(valor, salt);

        return new Senha(hash, salt);
    }

    public static Senha Restaurar(string hash, string salt) => new(hash, salt);

    public bool Verificar(string valor) => HashPassword(valor, Salt) == Hash;

    private static string GenerateSalt()
    {
        Span<byte> saltBytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    private static string HashPassword(string senha, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, saltBytes, 100_000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(hash);
    }
}