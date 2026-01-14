namespace BankingServiceProject.SharedProject.Security.Secrets;

public class EnvironmentSecretsProvider : ISecretsProvider
{
    public string GetSecretString(string name)
    {
        string? value = Environment.GetEnvironmentVariable(name);

        return !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new InvalidOperationException($"Key '{name}' not found in environment variables.");
    }

    public byte[] GetSecretByteArray(string name)
    {
        string value = GetSecretString(name);
        return Convert.FromBase64String(value);
    }
}