namespace BankingServiceProject.CommonProject.Security.Secrets;

public interface ISecretsProvider
{
    string GetSecretString(string name);

    byte[] GetSecretByteArray(string name);
}