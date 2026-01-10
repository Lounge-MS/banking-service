namespace BankingServiceProject.Cryptography;

public interface ISecretKeyProvider
{
    byte[] GetSecretKey(string name);
}