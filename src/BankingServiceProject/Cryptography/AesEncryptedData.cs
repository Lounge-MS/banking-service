namespace BankingServiceProject.Cryptography;

public record AesEncryptedData(
    byte[] EncryptedData,
    byte[] Iv);