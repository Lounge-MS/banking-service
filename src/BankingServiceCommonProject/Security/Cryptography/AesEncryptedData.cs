namespace BankingServiceProject.CommonProject.Security.Cryptography;

public record AesEncryptedData(
    byte[] EncryptedData,
    byte[] Iv);