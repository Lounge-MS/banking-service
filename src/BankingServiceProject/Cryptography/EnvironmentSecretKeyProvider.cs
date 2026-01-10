namespace BankingServiceProject.Cryptography;

public class EnvironmentSecretKeyProvider : ISecretKeyProvider
{
    public byte[] GetSecretKey(string name)
    {
        string? value = Environment.GetEnvironmentVariable(name);

        return !string.IsNullOrWhiteSpace(value)
            ? Convert.FromBase64String(value)
            : throw new InvalidOperationException($"Key '{name}' not found in environment variables.");
    }
}