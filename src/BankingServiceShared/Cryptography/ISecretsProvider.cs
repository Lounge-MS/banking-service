namespace BankingServiceProject.SharedProject.Cryptography;

public interface ISecretsProvider
{
    string GetSecretString(string name);

    byte[] GetSecretByteArray(string name);
}