namespace BankingServiceProject.SharedProject.Security.Secrets;

public interface ISecretsProvider
{
    string GetSecretString(string name);

    byte[] GetSecretByteArray(string name);
}